using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Orders;
using ShoppingOnline.Service.Orders;

namespace ShoppingOnline.Handler.Orders;

public interface IGetOrdersHandler : IBaseHandler<GetOrdersRequest, ServiceResponse>;

public class GetOrdersHandler(ILogger logger, IOrderService service)
    : BaseHandler<IOrderService, GetOrdersRequest, ServiceResponse>(logger, service), IGetOrdersHandler
{
    public override async Task<ServiceResponse> Handler(GetOrdersRequest request)
        => await service.GetOrdersAsync(request);
}
