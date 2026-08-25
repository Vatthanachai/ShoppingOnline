using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Orders;
using ShoppingOnline.Service.Orders;

namespace ShoppingOnline.Handler.Orders;

public interface ICancelOrderHandler : IBaseHandler<CancelOrderRequest, ServiceResponse>;

public class CancelOrderHandler(ILogger logger, IOrderService service)
    : BaseHandler<IOrderService, CancelOrderRequest, ServiceResponse>(logger, service), ICancelOrderHandler
{
    public override async Task<ServiceResponse> Handler(CancelOrderRequest request)
        => await service.CancelOrderAsync(request);
}
