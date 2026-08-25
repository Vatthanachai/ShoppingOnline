using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Dto.Auth;
using ShoppingOnline.Service.Auth;

namespace ShoppingOnline.Handler.Auth;

public interface ISignInHandler : IBaseHandler<SignInRequest, ServiceResponse>
{
}

public class SignInHandler(ILogger logger, IAuthService service)
    : BaseHandler<IAuthService, SignInRequest, ServiceResponse>(logger, service), ISignInHandler
{
    public override async Task<ServiceResponse> Handler(SignInRequest request)
        => await service.SignInAsync(request);
}
