using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Service.ProductCategories;

namespace ShoppingOnline.Handler.ProductCategories;

public interface ICreateCategoryHandler : IBaseHandler<CreateCategoryRequest, ServiceResponse>;

public class CreateCategoryHandler(ILogger logger, IProductCategoryService service)
    : BaseHandler<IProductCategoryService, CreateCategoryRequest, ServiceResponse>(logger, service),
        ICreateCategoryHandler
{
    public override async Task<ServiceResponse> Handler(CreateCategoryRequest request)
        => await service.CreateCategoryAsync(request);
}
