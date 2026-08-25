using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Products;

namespace ShoppingOnline.Service.Products;

public interface IProductService : IBaseService<Product>
{
    Task<ServiceResponse> GetProductsAsync(GetProductsRequest request);
    Task<ServiceResponse> GetProductAsync(GetProductRequest request);
    Task<ServiceResponse> CreateProductAsync(CreateProductRequest request);
    Task<ServiceResponse> UpdateProductAsync(UpdateProductRequest request);
    Task<ServiceResponse> DeactivateProductAsync(DeactivateProductRequest request);
}
