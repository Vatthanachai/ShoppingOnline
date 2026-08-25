using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Service.Stocks;

namespace ShoppingOnline.Handler.Stocks;

public interface IGetStocksHandler : IBaseHandler<GetStocksRequest, ServiceResponse>;

public class GetStocksHandler(ILogger logger, IStockService service)
    : BaseHandler<IStockService, GetStocksRequest, ServiceResponse>(logger, service), IGetStocksHandler
{
    public override async Task<ServiceResponse> Handler(GetStocksRequest request)
        => await service.GetStocksAsync(request);
}
