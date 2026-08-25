using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Orders;

namespace ShoppingOnline.Service.Orders;

public interface IOrderService : IBaseService<Order>
{
    Task<ServiceResponse> GetOrdersAsync(GetOrdersRequest request);
    Task<ServiceResponse> GetOrderAsync(GetOrderRequest request);
    Task<ServiceResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<ServiceResponse> CancelOrderAsync(CancelOrderRequest request);
}
