using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Service.PurchaseOrders;

namespace ShoppingOnline.Handler.PurchaseOrders;

public interface ISendPurchaseOrderHandler : IBaseHandler<SendPurchaseOrderRequest, ServiceResponse>;

public class SendPurchaseOrderHandler(ILogger logger, IPurchaseOrderService service)
    : BaseHandler<IPurchaseOrderService, SendPurchaseOrderRequest, ServiceResponse>(logger, service), ISendPurchaseOrderHandler
{
    public override async Task<ServiceResponse> Handler(SendPurchaseOrderRequest request)
        => await service.SendPurchaseOrderAsync(request);
}
