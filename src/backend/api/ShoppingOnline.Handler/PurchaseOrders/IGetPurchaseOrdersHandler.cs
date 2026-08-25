using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Service.PurchaseOrders;

namespace ShoppingOnline.Handler.PurchaseOrders;

public interface IGetPurchaseOrdersHandler : IBaseHandler<GetPurchaseOrdersRequest, ServiceResponse>;

public class GetPurchaseOrdersHandler(ILogger logger, IPurchaseOrderService service)
    : BaseHandler<IPurchaseOrderService, GetPurchaseOrdersRequest, ServiceResponse>(logger, service), IGetPurchaseOrdersHandler
{
    public override async Task<ServiceResponse> Handler(GetPurchaseOrdersRequest request)
        => await service.GetPurchaseOrdersAsync(request);
}
