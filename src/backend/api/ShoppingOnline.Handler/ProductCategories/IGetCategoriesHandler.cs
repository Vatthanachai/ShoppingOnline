using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Service.ProductCategories;

namespace ShoppingOnline.Handler.ProductCategories;

public interface IGetCategoriesHandler : IBaseHandler<GetCategoriesRequest, ServiceResponse>;

public class GetCategoriesHandler(ILogger logger, IProductCategoryService service)
    : BaseHandler<IProductCategoryService, GetCategoriesRequest, ServiceResponse>(logger, service),
        IGetCategoriesHandler
{
    public override async Task<ServiceResponse> Handler(GetCategoriesRequest request)
        => await service.GetCategoriesAsync(request);
}
