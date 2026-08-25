using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Emails;
using ShoppingOnline.Component.Abstractions.Emails.Templates;
using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Orders;
using ShoppingOnline.Model.Responses.Orders;

namespace ShoppingOnline.Service.Orders;

public class OrderService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailService emailService)
    : BaseService<Order, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IOrderService
{
    public async Task<ServiceResponse> GetOrdersAsync(GetOrdersRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var query = DbSet.Where(o => o.UserId == userId.Value)
            .Select(o => new GetOrdersResponse
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
            });

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.OrderId)
            : query.OrderBy(s => s.OrderId);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetOrderAsync(GetOrderRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var result = await MapToResponse(DbSet.Where(o => o.OrderId == request.OrderId && o.UserId == userId.Value))
            .FirstOrDefaultAsync();

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        // Only customers buy - admins manage the catalog/inventory, they don't place orders.
        if (httpContextAccessor.GetCurrentRole() == nameof(UserRole.Admin))
        {
            return new Service403Response("Admin accounts cannot place orders.");
        }

        if (request.Items is not { Count: > 0 })
        {
            return new Service400Response("At least one order item is required.");
        }

        var address = await DbContext.Set<ShippingAddress>()
            .FirstOrDefaultAsync(a => a.ShippingAddressId == request.ShippingAddressId && a.UserId == userId.Value);
        if (address is null)
        {
            return new Service400Response("Shipping address not found.");
        }

        // Npgsql's configured retrying execution strategy refuses a plain
        // Database.BeginTransactionAsync() - it can only retry a transaction it owns end to
        // end, so the whole allocate-and-save unit has to run inside ExecuteAsync (mirrors
        // BaseUnitOfWork.CommitAsync's use of the same pattern for the normal SaveChanges path).
        var strategy = DbContext.Database.CreateExecutionStrategy();
        var response = await strategy.ExecuteAsync(async () =>
        {
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingAddressLine1 = address.AddressLine1,
                ShippingAddressLine2 = address.AddressLine2,
                ShippingCity = address.City,
                ShippingState = address.State,
                ShippingPostalCode = address.PostalCode,
                ShippingCountry = address.Country,
                CreatedBy = "system",
                CreatedOn = DateTime.UtcNow,
            };

            decimal totalAmount = 0;

            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        await transaction.RollbackAsync();
                        return (ServiceResponse)new Service400Response("Order item quantity must be greater than zero.");
                    }

                    var product = await DbContext.Set<Product>()
                        .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                    if (product is null || !product.IsActive)
                    {
                        await transaction.RollbackAsync();
                        return new Service404Response($"Product {item.ProductId} not found.");
                    }

                    var allocations = await AllocateFifoAsync(item.ProductId, item.Quantity);
                    if (allocations is null)
                    {
                        await transaction.RollbackAsync();
                        return new Service400Response($"Insufficient stock for product \"{product.ProductName}\".");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.SellPrice,
                        TaxRatePercent = product.TaxRatePercent,
                        CreatedBy = "system",
                        CreatedOn = DateTime.UtcNow,
                    };

                    foreach (var allocation in allocations)
                    {
                        orderItem.Allocations.Add(new OrderItemAllocation
                        {
                            StockId = allocation.StockId,
                            VendorId = allocation.VendorId,
                            Quantity = allocation.Quantity,
                        });
                    }

                    order.OrderItems.Add(orderItem);
                    totalAmount += Math.Round(item.Quantity * product.SellPrice * (1 + product.TaxRatePercent / 100), 2);
                }

                order.TotalAmount = totalAmount;
                DbSet.Add(order);

                await DbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger.Error(ex, "Failed to create order for user {UserId}", userId);
                return new Service500Response(ex);
            }

            var created = await MapToResponse(DbSet.Where(o => o.OrderId == order.OrderId)).FirstOrDefaultAsync();
            return new Service200Response(created);
        });

        if (response is Service200Response { Data: GetOrderResponse orderResponse })
        {
            await SendOrderConfirmationEmailAsync(userId.Value, orderResponse);
        }

        return response;
    }

    /// <summary>
    /// Consumes stock lots for a product oldest-first (by CreatedOn), across every vendor that
    /// has supplied it. Each lot's decrement is a single conditional UPDATE (quantity -= take
    /// WHERE quantity >= take) so it stays safe under concurrent orders without needing an
    /// explicit row lock - Postgres serializes the statement itself. Returns null (and leaves
    /// whatever was already decremented for the caller to roll back) if the product doesn't
    /// have enough stock across all its lots.
    /// </summary>
    private async Task<List<(int StockId, int VendorId, int Quantity)>?> AllocateFifoAsync(int productId, int quantityNeeded)
    {
        var allocations = new List<(int StockId, int VendorId, int Quantity)>();
        var remaining = quantityNeeded;

        var lots = await DbContext.Set<Stock>()
            .Where(s => s.ProductId == productId && s.Quantity > 0)
            .OrderBy(s => s.CreatedOn)
            .Select(s => new { s.StockId, s.VendorId, s.Quantity })
            .ToListAsync();

        foreach (var lot in lots)
        {
            if (remaining <= 0) break;

            var take = Math.Min(remaining, lot.Quantity);

            var rowsAffected = await DbContext.Set<Stock>()
                .Where(s => s.StockId == lot.StockId && s.Quantity >= take)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Quantity, s => s.Quantity - take));

            if (rowsAffected == 0)
            {
                // Someone else drained this lot between our read and our update - bail out
                // rather than risk an inconsistent partial allocation; the customer can retry.
                return null;
            }

            allocations.Add((lot.StockId, lot.VendorId, take));
            remaining -= take;
        }

        return remaining == 0 ? allocations : null;
    }

    /// <summary>
    /// Best-effort: the order is already committed by the time this runs, so a failure here
    /// (e.g. SMTP down) is logged but must not turn a successful purchase into an error response.
    /// </summary>
    private async Task SendOrderConfirmationEmailAsync(int userId, GetOrderResponse order)
    {
        try
        {
            var userEmail = await DbContext.Set<User>()
                .Where(u => u.UserId == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return;
            }

            var items = order.Items
                .Select(i => new OrderConfirmationEmailTemplate.Item(i.ProductName, i.Quantity, i.LineTotal))
                .ToList();

            var address = string.Join(" ", new[]
            {
                order.ShippingAddressLine1, order.ShippingAddressLine2, order.ShippingCity,
                order.ShippingState, order.ShippingPostalCode, order.ShippingCountry,
            }.Where(s => !string.IsNullOrWhiteSpace(s)));

            var body = OrderConfirmationEmailTemplate.Build(order.OrderId, order.OrderDate, items, order.TotalAmount, address);

            await emailService.SendAsync(userEmail, $"ยืนยันคำสั่งซื้อ #{order.OrderId} - ShoppingOnline", body);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send order confirmation email for order {OrderId}", order.OrderId);
        }
    }

    public async Task<ServiceResponse> CancelOrderAsync(CancelOrderRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var order = await DbSet.Include(o => o.OrderItems).ThenInclude(i => i.Allocations)
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.UserId == userId.Value);

        if (order is null) return new Service404Response();

        if (order.Status != OrderStatus.Pending)
        {
            return new Service409Response("Only pending orders can be cancelled.");
        }

        foreach (var allocation in order.OrderItems.SelectMany(i => i.Allocations))
        {
            await DbContext.Set<Stock>()
                .Where(s => s.StockId == allocation.StockId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Quantity, s => s.Quantity + allocation.Quantity));
        }

        order.Status = OrderStatus.Cancelled;
        order.ModifiedBy = "system";
        order.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to cancel the order."));
        }

        return new Service200Response("Order cancelled successfully.");
    }

    private static IQueryable<GetOrderResponse> MapToResponse(IQueryable<Order> query)
        => query.Select(o => new GetOrderResponse
        {
            OrderId = o.OrderId,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ShippingAddressLine1 = o.ShippingAddressLine1,
            ShippingAddressLine2 = o.ShippingAddressLine2,
            ShippingCity = o.ShippingCity,
            ShippingState = o.ShippingState,
            ShippingPostalCode = o.ShippingPostalCode,
            ShippingCountry = o.ShippingCountry,
            Items = o.OrderItems.Select(i => new OrderItemResponse
            {
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TaxRatePercent = i.TaxRatePercent,
                LineTotal = Math.Round(i.Quantity * i.UnitPrice * (1 + i.TaxRatePercent / 100), 2),
            }).ToList(),
        });
}
