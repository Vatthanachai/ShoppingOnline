using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Stocks;

namespace ShoppingOnline.Service.Stocks;

public interface IStockService : IBaseService<Stock>
{
    Task<ServiceResponse> GetStocksAsync(GetStocksRequest request);
    Task<ServiceResponse> GetStockAsync(GetStockRequest request);
    Task<ServiceResponse> CreateStockAsync(CreateStockRequest request);
    Task<ServiceResponse> UpdateStockAsync(UpdateStockRequest request);
    Task<ServiceResponse> DeleteStockAsync(DeleteStockRequest request);
}
