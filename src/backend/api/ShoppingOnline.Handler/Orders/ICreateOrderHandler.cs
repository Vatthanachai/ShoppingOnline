using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Orders;
using ShoppingOnline.Service.Orders;

namespace ShoppingOnline.Handler.Orders;

public interface ICreateOrderHandler : IBaseHandler<CreateOrderRequest, ServiceResponse>;

public class CreateOrderHandler(ILogger logger, IOrderService service)
    : BaseHandler<IOrderService, CreateOrderRequest, ServiceResponse>(logger, service), ICreateOrderHandler
{
    public override async Task<ServiceResponse> Handler(CreateOrderRequest request)
        => await service.CreateOrderAsync(request);
}
