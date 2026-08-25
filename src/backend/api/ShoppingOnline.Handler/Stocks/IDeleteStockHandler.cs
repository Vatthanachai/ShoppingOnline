using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Stocks;
using ShoppingOnline.Service.Stocks;

namespace ShoppingOnline.Handler.Stocks;

public interface IDeleteStockHandler : IBaseHandler<DeleteStockRequest, ServiceResponse>;

public class DeleteStockHandler(ILogger logger, IStockService service)
    : BaseHandler<IStockService, DeleteStockRequest, ServiceResponse>(logger, service), IDeleteStockHandler
{
    public override async Task<ServiceResponse> Handler(DeleteStockRequest request)
        => await service.DeleteStockAsync(request);
}
