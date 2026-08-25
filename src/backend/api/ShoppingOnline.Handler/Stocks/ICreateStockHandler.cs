using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Service.Stocks;

namespace ShoppingOnline.Handler.Stocks;

public interface ICreateStockHandler : IBaseHandler<CreateStockRequest, ServiceResponse>;

public class CreateStockHandler(ILogger logger, IStockService service)
    : BaseHandler<IStockService, CreateStockRequest, ServiceResponse>(logger, service), ICreateStockHandler
{
    public override async Task<ServiceResponse> Handler(CreateStockRequest request)
        => await service.CreateStockAsync(request);
}
