using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IDeactivateUserHandler : IBaseHandler<DeactivateUserRequest, ServiceResponse>;

public class DeactivateUserHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, DeactivateUserRequest, ServiceResponse>(logger, service), IDeactivateUserHandler
{
    public override async Task<ServiceResponse> Handler(DeactivateUserRequest request)
        => await service.DeactivateUserAsync(request);
}
