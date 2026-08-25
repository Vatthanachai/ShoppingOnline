using System.Linq.Expressions;

using Mapster;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Model.Responses.Categories;

namespace ShoppingOnline.Service.ProductCategories;

public class ProductCategoryService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : BaseService<ProductCategory, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IProductCategoryService
{
    public async Task<ServiceResponse> GetCategoriesAsync(GetCategoriesRequest request)
    {
        Expression<Func<GetCategoriesResponse, bool>> predicate = s => true;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = predicate.And(s =>
                s.CategoryName.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase) ||
                s.Description.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        var query = DbContext.Set<ProductCategory>().ProjectToType<GetCategoriesResponse>().Where(predicate);

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.CategoryName)
            : query.OrderBy(s => s.CategoryName);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetCategoryAsync(GetCategoryRequest request)
    {
        var result = await DbContext.Set<ProductCategory>().ProjectToType<GetCategoryResponse>()
            .FirstOrDefaultAsync(c => c.ProductCategoryId == request.CategoryId);

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var nameExists = await DbSet.AnyAsync(c => c.CategoryName == request.CategoryName);
        if (nameExists)
        {
            return new Service409Response("A category with this name already exists.");
        }

        var category = new ProductCategory
        {
            CategoryName = request.CategoryName,
            Description = request.Description,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
        };

        DbSet.Add(category);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the category."));
        }

        return new Service200Response(category.Adapt<GetCategoryResponse>());
    }

    public async Task<ServiceResponse> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var category = await DbSet.FirstOrDefaultAsync(c => c.ProductCategoryId == request.CategoryId);
        if (category is null) return new Service404Response();

        var nameExists = await DbSet.AnyAsync(c =>
            c.CategoryName == request.CategoryName && c.ProductCategoryId != request.CategoryId);
        if (nameExists)
        {
            return new Service409Response("A category with this name already exists.");
        }

        category.CategoryName = request.CategoryName;
        category.Description = request.Description;
        category.ModifiedBy = "system";
        category.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the category."));
        }

        return new Service200Response(category.Adapt<GetCategoryResponse>());
    }

    public async Task<ServiceResponse> DeactivateCategoryAsync(DeactivateCategoryRequest request)
    {
        var category = await DbSet.FirstOrDefaultAsync(c => c.ProductCategoryId == request.CategoryId);
        if (category is null) return new Service404Response();

        category.IsActive = false;
        category.ModifiedBy = "system";
        category.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to deactivate the category."));
        }

        return new Service200Response("Category deactivated successfully.");
    }
}