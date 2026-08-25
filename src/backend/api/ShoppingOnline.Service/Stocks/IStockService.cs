using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Stocks;

namespace ShoppingOnline.Service.Stocks;

/// <summary>
/// Read-only inventory view. Stock lots are only ever created via a received Purchase Order
/// (see PurchaseOrderService.ReceiveAsync) and consumed FIFO by OrderService - there's no
/// direct create/update/delete here anymore.
/// </summary>
public interface IStockService : IBaseService<Stock>
{
    Task<ServiceResponse> GetStocksAsync(GetStocksRequest request);
    Task<ServiceResponse> GetStockAsync(GetStockRequest request);
}
