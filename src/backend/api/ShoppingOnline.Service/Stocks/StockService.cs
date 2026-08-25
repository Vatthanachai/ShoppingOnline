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
            ? query.OrderByDescending(s => s.StockId)
            : query.OrderBy(s => s.StockId);
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

    public async Task<ServiceResponse> CreateStockAsync(CreateStockRequest request)
    {
        var productExists = await DbContext.Set<Product>().AnyAsync(p => p.ProductId == request.ProductId);
        if (!productExists)
        {
            return new Service404Response("Product not found.");
        }

        var vendorExists = await DbContext.Set<Vendor>().AnyAsync(v => v.VendorId == request.VendorId);
        if (!vendorExists)
        {
            return new Service404Response("Vendor not found.");
        }

        var stockExists = await DbSet.AnyAsync(s => s.ProductId == request.ProductId && s.VendorId == request.VendorId);
        if (stockExists)
        {
            return new Service409Response("Stock for this product and vendor already exists. Use update instead.");
        }

        var stock = new Stock
        {
            ProductId = request.ProductId,
            VendorId = request.VendorId,
            Quantity = request.Quantity,
            Price = request.Price,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
        };

        DbSet.Add(stock);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the stock."));
        }

        var result = await MapToResponse(DbSet.Where(s => s.StockId == stock.StockId)).FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> UpdateStockAsync(UpdateStockRequest request)
    {
        var stock = await DbSet.FirstOrDefaultAsync(s => s.StockId == request.StockId);
        if (stock is null) return new Service404Response();

        stock.Quantity = request.Quantity;
        stock.Price = request.Price;
        stock.ModifiedBy = "system";
        stock.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the stock."));
        }

        var result = await MapToResponse(DbSet.Where(s => s.StockId == stock.StockId)).FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> DeleteStockAsync(DeleteStockRequest request)
    {
        var stock = await DbSet.FirstOrDefaultAsync(s => s.StockId == request.StockId);
        if (stock is null) return new Service404Response();

        DbSet.Remove(stock);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to delete the stock."));
        }

        return new Service200Response("Stock deleted successfully.");
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
            Price = s.Price,
        });
}
