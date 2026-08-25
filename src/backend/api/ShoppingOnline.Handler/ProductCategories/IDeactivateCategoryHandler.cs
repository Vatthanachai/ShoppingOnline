using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Service.ProductCategories;

namespace ShoppingOnline.Handler.ProductCategories;

public interface IDeactivateCategoryHandler : IBaseHandler<DeactivateCategoryRequest, ServiceResponse>;

public class DeactivateCategoryHandler(ILogger logger, IProductCategoryService service)
    : BaseHandler<IProductCategoryService, DeactivateCategoryRequest, ServiceResponse>(logger, service),
        IDeactivateCategoryHandler
{
    public override async Task<ServiceResponse> Handler(DeactivateCategoryRequest request)
        => await service.DeactivateCategoryAsync(request);
}
