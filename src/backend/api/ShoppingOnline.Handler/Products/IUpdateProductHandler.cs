using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Service.Products;

namespace ShoppingOnline.Handler.Products;

public interface IUpdateProductHandler : IBaseHandler<UpdateProductRequest, ServiceResponse>;

public class UpdateProductHandler(ILogger logger, IProductService service)
    : BaseHandler<IProductService, UpdateProductRequest, ServiceResponse>(logger, service), IUpdateProductHandler
{
    public override async Task<ServiceResponse> Handler(UpdateProductRequest request)
        => await service.UpdateProductAsync(request);
}
