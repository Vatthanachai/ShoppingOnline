using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Service.PurchaseOrders;

namespace ShoppingOnline.Handler.PurchaseOrders;

public interface IGetPurchaseOrderHandler : IBaseHandler<GetPurchaseOrderRequest, ServiceResponse>;

public class GetPurchaseOrderHandler(ILogger logger, IPurchaseOrderService service)
    : BaseHandler<IPurchaseOrderService, GetPurchaseOrderRequest, ServiceResponse>(logger, service), IGetPurchaseOrderHandler
{
    public override async Task<ServiceResponse> Handler(GetPurchaseOrderRequest request)
        => await service.GetPurchaseOrderAsync(request);
}
