using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Dto.Auth;
using ShoppingOnline.Service.Auth;

namespace ShoppingOnline.Handler.Auth;

public interface ISignUpHandler : IBaseHandler<SignUpRequest, ServiceResponse>
{
}

public class SignUpHandler(ILogger logger, IAuthService service)
    : BaseHandler<IAuthService, SignUpRequest, ServiceResponse>(logger, service), ISignUpHandler
{
    public override async Task<ServiceResponse> Handler(SignUpRequest request)
        => await service.SignUpAsync(request);
}