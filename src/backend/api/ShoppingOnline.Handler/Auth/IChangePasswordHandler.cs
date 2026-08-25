using Microsoft.AspNetCore.Http;

using Serilog;

using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Dto.Auth;
using ShoppingOnline.Service.Auth;

namespace ShoppingOnline.Handler.Auth;

public interface IChangePasswordHandler : IBaseHandler<ChangePasswordRequest, ServiceResponse>
{
}

public class ChangePasswordHandler(ILogger logger, IAuthService service, IHttpContextAccessor httpContextAccessor)
    : BaseHandler<IAuthService, ChangePasswordRequest, ServiceResponse>(logger, service), IChangePasswordHandler
{
    public override async Task<ServiceResponse> Handler(ChangePasswordRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        return await service.ChangePasswordAsync(userId.Value, request);
    }
}
