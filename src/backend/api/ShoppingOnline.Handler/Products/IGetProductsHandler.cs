using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Service.Products;

namespace ShoppingOnline.Handler.Products;

public interface IGetProductsHandler : IBaseHandler<GetProductsRequest, ServiceResponse>;

public class GetProductsHandler(ILogger logger, IProductService service)
    : BaseHandler<IProductService, GetProductsRequest, ServiceResponse>(logger, service), IGetProductsHandler
{
    public override async Task<ServiceResponse> Handler(GetProductsRequest request)
        => await service.GetProductsAsync(request);
}
