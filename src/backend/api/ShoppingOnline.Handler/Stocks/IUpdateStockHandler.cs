using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Service.Stocks;

namespace ShoppingOnline.Handler.Stocks;

public interface IUpdateStockHandler : IBaseHandler<UpdateStockRequest, ServiceResponse>;

public class UpdateStockHandler(ILogger logger, IStockService service)
    : BaseHandler<IStockService, UpdateStockRequest, ServiceResponse>(logger, service), IUpdateStockHandler
{
    public override async Task<ServiceResponse> Handler(UpdateStockRequest request)
        => await service.UpdateStockAsync(request);
}
