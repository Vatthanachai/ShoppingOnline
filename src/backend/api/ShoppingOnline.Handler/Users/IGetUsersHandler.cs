using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IGetUsersHandler : IBaseHandler<GetUsersRequest, ServiceResponse>;

public class GetUsersHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, GetUsersRequest, ServiceResponse>(logger, service), IGetUsersHandler
{
    public override async Task<ServiceResponse> Handler(GetUsersRequest request)
        => await service.GetUsersAsync(request);
}
