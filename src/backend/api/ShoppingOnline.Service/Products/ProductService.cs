using System.Linq.Expressions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Model.Responses.Products;

namespace ShoppingOnline.Service.Products;

public class ProductService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : BaseService<Product, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IProductService
{
    public async Task<ServiceResponse> GetProductsAsync(GetProductsRequest request)
    {
        Expression<Func<Product, bool>> predicate = p => true;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = predicate.And(p =>
                p.ProductName.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase) ||
                p.ProductCode.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        if (request.ProductCategoryId.HasValue)
        {
            predicate = predicate.And(p => p.ProductCategoryId == request.ProductCategoryId.Value);
        }

        if (request.VendorId.HasValue)
        {
            predicate = predicate.And(p => p.VendorId == request.VendorId.Value);
        }

        var baseQuery = DbSet.Where(predicate);

        var query = MapToResponse(baseQuery);

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.ProductName)
            : query.OrderBy(s => s.ProductName);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetProductAsync(GetProductRequest request)
    {
        var result = await MapToResponse(DbSet.Where(p => p.ProductId == request.ProductId))
            .FirstOrDefaultAsync();

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreateProductAsync(CreateProductRequest request)
    {
        var categoryExists = await DbContext.Set<ProductCategory>().AnyAsync(c => c.ProductCategoryId == request.ProductCategoryId);
        if (!categoryExists)
        {
            return new Service404Response("Product category not found.");
        }

        var vendorExists = await DbContext.Set<Vendor>().AnyAsync(v => v.VendorId == request.VendorId);
        if (!vendorExists)
        {
            return new Service404Response("Vendor not found.");
        }

        var codeExists = await DbSet.AnyAsync(p => p.ProductCode == request.ProductCode);
        if (codeExists)
        {
            return new Service409Response("A product with this product code already exists.");
        }

        var product = new Product
        {
            ProductCategoryId = request.ProductCategoryId,
            VendorId = request.VendorId,
            ProductCode = request.ProductCode,
            ProductName = request.ProductName,
            Description = request.Description,
            ImagePath = request.ImagePath,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
        };

        DbSet.Add(product);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the product."));
        }

        var result = await MapToResponse(DbSet.Where(p => p.ProductId == product.ProductId)).FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await DbSet.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        if (product is null) return new Service404Response();

        var categoryExists = await DbContext.Set<ProductCategory>().AnyAsync(c => c.ProductCategoryId == request.ProductCategoryId);
        if (!categoryExists)
        {
            return new Service404Response("Product category not found.");
        }

        var vendorExists = await DbContext.Set<Vendor>().AnyAsync(v => v.VendorId == request.VendorId);
        if (!vendorExists)
        {
            return new Service404Response("Vendor not found.");
        }

        var codeExists = await DbSet.AnyAsync(p => p.ProductCode == request.ProductCode && p.ProductId != request.ProductId);
        if (codeExists)
        {
            return new Service409Response("A product with this product code already exists.");
        }

        product.ProductCategoryId = request.ProductCategoryId;
        product.VendorId = request.VendorId;
        product.ProductCode = request.ProductCode;
        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.ImagePath = request.ImagePath;
        product.ModifiedBy = "system";
        product.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the product."));
        }

        var result = await MapToResponse(DbSet.Where(p => p.ProductId == product.ProductId)).FirstOrDefaultAsync();
        return new Service200Response(result);
    }

    public async Task<ServiceResponse> DeactivateProductAsync(DeactivateProductRequest request)
    {
        var product = await DbSet.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        if (product is null) return new Service404Response();

        product.IsActive = false;
        product.ModifiedBy = "system";
        product.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to deactivate the product."));
        }

        return new Service200Response("Product deactivated successfully.");
    }

    private static IQueryable<GetProductResponse> MapToResponse(IQueryable<Product> query)
        => query.Select(p => new GetProductResponse
        {
            ProductId = p.ProductId,
            ProductCategoryId = p.ProductCategoryId,
            ProductCategoryName = p.ProductCategory.CategoryName,
            VendorId = p.VendorId,
            VendorName = p.Vendor.VendorName,
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            Description = p.Description,
            IsActive = p.IsActive,
            MinPrice = p.Stocks.Where(s => s.Quantity > 0).Select(s => (decimal?)s.Price).Min(),
            ImagePath = p.ImagePath,
        });
}
