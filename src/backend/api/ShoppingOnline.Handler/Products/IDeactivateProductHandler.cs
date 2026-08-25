using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Service.Products;

namespace ShoppingOnline.Handler.Products;

public interface IDeactivateProductHandler : IBaseHandler<DeactivateProductRequest, ServiceResponse>;

public class DeactivateProductHandler(ILogger logger, IProductService service)
    : BaseHandler<IProductService, DeactivateProductRequest, ServiceResponse>(logger, service),
        IDeactivateProductHandler
{
    public override async Task<ServiceResponse> Handler(DeactivateProductRequest request)
        => await service.DeactivateProductAsync(request);
}
