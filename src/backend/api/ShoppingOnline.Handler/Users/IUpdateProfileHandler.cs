using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IUpdateProfileHandler : IBaseHandler<UpdateProfileRequest, ServiceResponse>;

public class UpdateProfileHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, UpdateProfileRequest, ServiceResponse>(logger, service), IUpdateProfileHandler
{
    public override async Task<ServiceResponse> Handler(UpdateProfileRequest request)
        => await service.UpdateProfileAsync(request);
}
