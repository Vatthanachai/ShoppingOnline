using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

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
    IHttpContextAccessor httpContextAccessor)
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

        if (request.Items is not { Count: > 0 })
        {
            return new Service400Response("At least one order item is required.");
        }

        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
        };

        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return new Service400Response("Order item quantity must be greater than zero.");
            }

            var stock = await DbContext.Set<Stock>()
                .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.VendorId == item.VendorId);

            if (stock is null)
            {
                return new Service404Response($"Stock not found for product {item.ProductId} from vendor {item.VendorId}.");
            }

            if (stock.Quantity < item.Quantity)
            {
                return new Service400Response($"Insufficient stock for product {item.ProductId} from vendor {item.VendorId}.");
            }

            stock.Quantity -= item.Quantity;
            stock.ModifiedBy = "system";
            stock.ModifiedDate = DateTime.UtcNow;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                VendorId = item.VendorId,
                Quantity = item.Quantity,
                Price = stock.Price,
                CreatedBy = "system",
                CreatedOn = DateTime.UtcNow,
            });

            totalAmount += stock.Price * item.Quantity;
        }

        order.TotalAmount = totalAmount;

        DbSet.Add(order);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the order."));
        }

        var result = await MapToResponse(DbSet.Where(o => o.OrderId == order.OrderId)).FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CancelOrderAsync(CancelOrderRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var order = await DbSet.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.UserId == userId.Value);

        if (order is null) return new Service404Response();

        if (order.Status != OrderStatus.Pending)
        {
            return new Service409Response("Only pending orders can be cancelled.");
        }

        foreach (var item in order.OrderItems)
        {
            var stock = await DbContext.Set<Stock>()
                .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.VendorId == item.VendorId);

            if (stock is not null)
            {
                stock.Quantity += item.Quantity;
                stock.ModifiedBy = "system";
                stock.ModifiedDate = DateTime.UtcNow;
            }
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
            Items = o.OrderItems.Select(i => new OrderItemResponse
            {
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                VendorId = i.VendorId,
                VendorName = i.Vendor.VendorName,
                Quantity = i.Quantity,
                Price = i.Price,
            }).ToList(),
        });
}
