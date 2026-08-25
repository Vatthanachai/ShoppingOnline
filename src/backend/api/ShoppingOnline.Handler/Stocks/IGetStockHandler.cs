using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Service.Stocks;

namespace ShoppingOnline.Handler.Stocks;

public interface IGetStockHandler : IBaseHandler<GetStockRequest, ServiceResponse>;

public class GetStockHandler(ILogger logger, IStockService service)
    : BaseHandler<IStockService, GetStockRequest, ServiceResponse>(logger, service), IGetStockHandler
{
    public override async Task<ServiceResponse> Handler(GetStockRequest request)
        => await service.GetStockAsync(request);
}
