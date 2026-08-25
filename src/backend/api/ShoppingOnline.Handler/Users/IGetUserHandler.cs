using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IGetUserHandler : IBaseHandler<GetUserRequest, ServiceResponse>;

public class GetUserHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, GetUserRequest, ServiceResponse>(logger, service), IGetUserHandler
{
    public override async Task<ServiceResponse> Handler(GetUserRequest request)
        => await service.GetUserAsync(request);
}
