using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IActivateUserHandler : IBaseHandler<ActivateUserRequest, ServiceResponse>;

public class ActivateUserHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, ActivateUserRequest, ServiceResponse>(logger, service), IActivateUserHandler
{
    public override async Task<ServiceResponse> Handler(ActivateUserRequest request)
        => await service.ActivateUserAsync(request);
}
