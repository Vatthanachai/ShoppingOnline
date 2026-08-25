using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Categories;

namespace ShoppingOnline.Service.ProductCategories;

public interface IProductCategoryService : IBaseService<ProductCategory>
{
    Task<ServiceResponse> GetCategoriesAsync(GetCategoriesRequest request);
    Task<ServiceResponse> GetCategoryAsync(GetCategoryRequest request);
    Task<ServiceResponse> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ServiceResponse> UpdateCategoryAsync(UpdateCategoryRequest request);
    Task<ServiceResponse> DeactivateCategoryAsync(DeactivateCategoryRequest request);
}