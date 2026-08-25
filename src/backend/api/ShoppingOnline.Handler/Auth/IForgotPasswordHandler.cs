using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Dto.Auth;
using ShoppingOnline.Service.Auth;

namespace ShoppingOnline.Handler.Auth;

public interface IForgotPasswordHandler : IBaseHandler<ForgotPasswordRequest, ServiceResponse>
{
}

public class ForgotPasswordHandler(ILogger logger, IAuthService service)
    : BaseHandler<IAuthService, ForgotPasswordRequest, ServiceResponse>(logger, service), IForgotPasswordHandler
{
    public override async Task<ServiceResponse> Handler(ForgotPasswordRequest request)
        => await service.ForgotPasswordAsync(request);
}
