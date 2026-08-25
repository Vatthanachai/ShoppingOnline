using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Model.Responses.Stocks;

namespace ShoppingOnline.Service.Stocks;

public class StockService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : BaseService<Stock, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IStockService
{
    public async Task<ServiceResponse> GetStocksAsync(GetStocksRequest request)
    {
        var baseQuery = DbSet.AsQueryable();

        if (request.ProductId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.ProductId == request.ProductId.Value);
        }

        if (request.VendorId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.VendorId == request.VendorId.Value);
        }

        var query = MapToResponse(baseQuery);

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.ReceivedOn)
            : query.OrderBy(s => s.ReceivedOn);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetStockAsync(GetStockRequest request)
    {
        var result = await MapToResponse(DbSet.Where(s => s.StockId == request.StockId)).FirstOrDefaultAsync();

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    private static IQueryable<GetStockResponse> MapToResponse(IQueryable<Stock> query)
        => query.Select(s => new GetStockResponse
        {
            StockId = s.StockId,
            ProductId = s.ProductId,
            ProductName = s.Product.ProductName,
            VendorId = s.VendorId,
            VendorName = s.Vendor.VendorName,
            Quantity = s.Quantity,
            Cost = s.Cost,
            ReceivedOn = s.CreatedOn,
            PurchaseOrderId = s.PurchaseOrderItem != null ? s.PurchaseOrderItem.PurchaseOrderId : null,
        });
}
