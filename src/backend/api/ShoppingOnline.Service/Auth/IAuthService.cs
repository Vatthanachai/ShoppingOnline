using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Dto.Auth;

namespace ShoppingOnline.Service.Auth;

public interface IAuthService
{
    Task<ServiceResponse> SignUpAsync(SignUpRequest request);

    Task<ServiceResponse> SignInAsync(SignInRequest request);

    /// <param name="userId">Identity of the currently signed-in user, resolved from the PASETO token by the caller</param>
    Task<ServiceResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);

    Task<ServiceResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
}
