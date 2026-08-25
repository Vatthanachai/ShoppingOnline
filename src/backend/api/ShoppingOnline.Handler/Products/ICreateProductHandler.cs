using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Products;
using ShoppingOnline.Service.Products;

namespace ShoppingOnline.Handler.Products;

public interface ICreateProductHandler : IBaseHandler<CreateProductRequest, ServiceResponse>;

public class CreateProductHandler(ILogger logger, IProductService service)
    : BaseHandler<IProductService, CreateProductRequest, ServiceResponse>(logger, service), ICreateProductHandler
{
    public override async Task<ServiceResponse> Handler(CreateProductRequest request)
        => await service.CreateProductAsync(request);
}
