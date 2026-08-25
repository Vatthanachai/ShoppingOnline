using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Emails;
using ShoppingOnline.Component.Abstractions.Emails.Templates;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Model.Responses.PurchaseOrders;

namespace ShoppingOnline.Service.PurchaseOrders;

public class PurchaseOrderService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailService emailService)
    : BaseService<PurchaseOrder, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IPurchaseOrderService
{
    public async Task<ServiceResponse> GetPurchaseOrdersAsync(GetPurchaseOrdersRequest request)
    {
        var baseQuery = DbSet.AsQueryable();

        if (request.VendorId.HasValue)
        {
            baseQuery = baseQuery.Where(p => p.VendorId == request.VendorId.Value);
        }

        if (request.Status.HasValue)
        {
            baseQuery = baseQuery.Where(p => p.Status == request.Status.Value);
        }

        var query = baseQuery.Select(p => new GetPurchaseOrdersResponse
        {
            PurchaseOrderId = p.PurchaseOrderId,
            VendorId = p.VendorId,
            VendorName = p.Vendor.VendorName,
            Status = p.Status,
            CreatedOn = p.CreatedOn,
            SentOn = p.SentOn,
            ItemCount = p.Items.Count,
        });

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.PurchaseOrderId)
            : query.OrderBy(s => s.PurchaseOrderId);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetPurchaseOrderAsync(GetPurchaseOrderRequest request)
    {
        var result = await MapToResponse(DbSet.Where(p => p.PurchaseOrderId == request.PurchaseOrderId))
            .FirstOrDefaultAsync();

        if (result is null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request)
    {
        if (request.Items is not { Count: > 0 })
        {
            return new Service400Response("At least one purchase order item is required.");
        }

        var vendor = await DbContext.Set<Vendor>().FirstOrDefaultAsync(v => v.VendorId == request.VendorId);
        if (vendor is null)
        {
            return new Service404Response("Vendor not found.");
        }

        var purchaseOrder = new PurchaseOrder
        {
            VendorId = request.VendorId,
            Status = PurchaseOrderStatus.Draft,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
        };

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return new Service400Response("Purchase order item quantity must be greater than zero.");
            }

            var productExists = await DbContext.Set<Product>().AnyAsync(p => p.ProductId == item.ProductId);
            if (!productExists)
            {
                return new Service404Response($"Product {item.ProductId} not found.");
            }

            purchaseOrder.Items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                QuantityOrdered = item.Quantity,
                QuantityReceived = 0,
                UnitCostQuoted = item.UnitCostQuoted,
            });
        }

        DbSet.Add(purchaseOrder);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the purchase order."));
        }

        var result = await MapToResponse(DbSet.Where(p => p.PurchaseOrderId == purchaseOrder.PurchaseOrderId))
            .FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> SendPurchaseOrderAsync(SendPurchaseOrderRequest request)
    {
        var purchaseOrder = await DbSet.Include(p => p.Vendor).Include(p => p.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == request.PurchaseOrderId);
        if (purchaseOrder is null) return new Service404Response();

        if (purchaseOrder.Status != PurchaseOrderStatus.Draft)
        {
            return new Service409Response("Only a draft purchase order can be sent.");
        }

        try
        {
            var items = purchaseOrder.Items
                .Select(i => new PurchaseOrderEmailTemplate.Item(i.Product.ProductCode, i.Product.ProductName,
                    i.QuantityOrdered, i.UnitCostQuoted))
                .ToList();

            var body = PurchaseOrderEmailTemplate.Build(purchaseOrder.PurchaseOrderId, purchaseOrder.Vendor.VendorName,
                purchaseOrder.CreatedOn, items);

            await emailService.SendAsync(purchaseOrder.Vendor.Email,
                $"ใบสั่งซื้อ PO-{purchaseOrder.PurchaseOrderId:D5} จาก ShoppingOnline", body);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send purchase order email for PO {PurchaseOrderId}", purchaseOrder.PurchaseOrderId);
            return new Service500Response(ex);
        }

        purchaseOrder.Status = PurchaseOrderStatus.Sent;
        purchaseOrder.SentOn = DateTime.UtcNow;
        purchaseOrder.ModifiedBy = "system";
        purchaseOrder.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the purchase order status."));
        }

        var result = await MapToResponse(DbSet.Where(p => p.PurchaseOrderId == purchaseOrder.PurchaseOrderId))
            .FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> ReceivePurchaseOrderAsync(ReceivePurchaseOrderRequest request)
    {
        if (request.Lines is not { Count: > 0 })
        {
            return new Service400Response("At least one line to receive is required.");
        }

        var purchaseOrder = await DbSet.Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == request.PurchaseOrderId);
        if (purchaseOrder is null) return new Service404Response();

        if (purchaseOrder.Status is not (PurchaseOrderStatus.Sent or PurchaseOrderStatus.PartiallyReceived))
        {
            return new Service409Response("Only a sent purchase order can receive stock.");
        }

        foreach (var line in request.Lines)
        {
            if (line.QuantityReceived <= 0)
            {
                return new Service400Response("Quantity received must be greater than zero.");
            }

            var item = purchaseOrder.Items.FirstOrDefault(i => i.PurchaseOrderItemId == line.PurchaseOrderItemId);
            if (item is null)
            {
                return new Service404Response($"Purchase order item {line.PurchaseOrderItemId} not found.");
            }

            item.QuantityReceived += line.QuantityReceived;

            DbContext.Set<Stock>().Add(new Stock
            {
                ProductId = item.ProductId,
                VendorId = purchaseOrder.VendorId,
                Quantity = line.QuantityReceived,
                Cost = line.UnitCost,
                PurchaseOrderItemId = item.PurchaseOrderItemId,
                CreatedBy = "system",
                CreatedOn = DateTime.UtcNow,
            });
        }

        purchaseOrder.Status = purchaseOrder.Items.All(i => i.QuantityReceived >= i.QuantityOrdered)
            ? PurchaseOrderStatus.Received
            : PurchaseOrderStatus.PartiallyReceived;
        purchaseOrder.ModifiedBy = "system";
        purchaseOrder.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to receive the purchase order."));
        }

        var result = await MapToResponse(DbSet.Where(p => p.PurchaseOrderId == purchaseOrder.PurchaseOrderId))
            .FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    private static IQueryable<GetPurchaseOrderResponse> MapToResponse(IQueryable<PurchaseOrder> query)
        => query.Select(p => new GetPurchaseOrderResponse
        {
            PurchaseOrderId = p.PurchaseOrderId,
            VendorId = p.VendorId,
            VendorName = p.Vendor.VendorName,
            VendorEmail = p.Vendor.Email,
            Status = p.Status,
            CreatedOn = p.CreatedOn,
            SentOn = p.SentOn,
            Items = p.Items.Select(i => new PurchaseOrderItemResponse
            {
                PurchaseOrderItemId = i.PurchaseOrderItemId,
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                ProductCode = i.Product.ProductCode,
                QuantityOrdered = i.QuantityOrdered,
                QuantityReceived = i.QuantityReceived,
                UnitCostQuoted = i.UnitCostQuoted,
            }).ToList(),
        });
}
