using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.PurchaseOrders;

namespace ShoppingOnline.Service.PurchaseOrders;

public interface IPurchaseOrderService : IBaseService<PurchaseOrder>
{
    Task<ServiceResponse> GetPurchaseOrdersAsync(GetPurchaseOrdersRequest request);
    Task<ServiceResponse> GetPurchaseOrderAsync(GetPurchaseOrderRequest request);
    Task<ServiceResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request);
    Task<ServiceResponse> SendPurchaseOrderAsync(SendPurchaseOrderRequest request);
    Task<ServiceResponse> ReceivePurchaseOrderAsync(ReceivePurchaseOrderRequest request);
}
