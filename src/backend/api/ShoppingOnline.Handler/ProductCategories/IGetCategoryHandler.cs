using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Categories;
using ShoppingOnline.Service.ProductCategories;

namespace ShoppingOnline.Handler.ProductCategories;

public interface IGetCategoryHandler : IBaseHandler<GetCategoryRequest, ServiceResponse>;

public class GetCategoryHandler(ILogger logger, IProductCategoryService service)
    : BaseHandler<IProductCategoryService, GetCategoryRequest, ServiceResponse>(logger, service), IGetCategoryHandler
{
    public override async Task<ServiceResponse> Handler(GetCategoryRequest request)
        => await service.GetCategoryAsync(request);
}
