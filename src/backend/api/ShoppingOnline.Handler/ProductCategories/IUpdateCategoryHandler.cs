using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Service.ProductCategories;

namespace ShoppingOnline.Handler.ProductCategories;

public interface IUpdateCategoryHandler : IBaseHandler<UpdateCategoryRequest, ServiceResponse>;

public class UpdateCategoryHandler(ILogger logger, IProductCategoryService service)
    : BaseHandler<IProductCategoryService, UpdateCategoryRequest, ServiceResponse>(logger, service),
        IUpdateCategoryHandler
{
    public override async Task<ServiceResponse> Handler(UpdateCategoryRequest request)
        => await service.UpdateCategoryAsync(request);
}
