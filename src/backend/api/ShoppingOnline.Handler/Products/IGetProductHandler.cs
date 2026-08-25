using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Service.Products;

namespace ShoppingOnline.Handler.Products;

public interface IGetProductHandler : IBaseHandler<GetProductRequest, ServiceResponse>;

public class GetProductHandler(ILogger logger, IProductService service)
    : BaseHandler<IProductService, GetProductRequest, ServiceResponse>(logger, service), IGetProductHandler
{
    public override async Task<ServiceResponse> Handler(GetProductRequest request)
        => await service.GetProductAsync(request);
}
