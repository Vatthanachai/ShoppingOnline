using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Service.PurchaseOrders;

namespace ShoppingOnline.Handler.PurchaseOrders;

public interface IReceivePurchaseOrderHandler : IBaseHandler<ReceivePurchaseOrderRequest, ServiceResponse>;

public class ReceivePurchaseOrderHandler(ILogger logger, IPurchaseOrderService service)
    : BaseHandler<IPurchaseOrderService, ReceivePurchaseOrderRequest, ServiceResponse>(logger, service), IReceivePurchaseOrderHandler
{
    public override async Task<ServiceResponse> Handler(ReceivePurchaseOrderRequest request)
        => await service.ReceivePurchaseOrderAsync(request);
}
