using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.PurchaseOrders;
using ShoppingOnline.Service.PurchaseOrders;

namespace ShoppingOnline.Handler.PurchaseOrders;

public interface ICreatePurchaseOrderHandler : IBaseHandler<CreatePurchaseOrderRequest, ServiceResponse>;

public class CreatePurchaseOrderHandler(ILogger logger, IPurchaseOrderService service)
    : BaseHandler<IPurchaseOrderService, CreatePurchaseOrderRequest, ServiceResponse>(logger, service), ICreatePurchaseOrderHandler
{
    public override async Task<ServiceResponse> Handler(CreatePurchaseOrderRequest request)
        => await service.CreatePurchaseOrderAsync(request);
}
