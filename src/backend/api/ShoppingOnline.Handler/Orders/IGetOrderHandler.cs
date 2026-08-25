using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Orders;
using ShoppingOnline.Service.Orders;

namespace ShoppingOnline.Handler.Orders;

public interface IGetOrderHandler : IBaseHandler<GetOrderRequest, ServiceResponse>;

public class GetOrderHandler(ILogger logger, IOrderService service)
    : BaseHandler<IOrderService, GetOrderRequest, ServiceResponse>(logger, service), IGetOrderHandler
{
    public override async Task<ServiceResponse> Handler(GetOrderRequest request)
        => await service.GetOrderAsync(request);
}
