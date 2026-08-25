using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Handler.Auth;
using ShoppingOnline.Model.Dto.Auth;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing user authorization-related operations, including sign-up, sign-in, password change, and password recovery.
/// </summary>
/// <param name="signUpHandler"></param>
/// <param name="signInHandler"></param>
/// <param name="changePasswordHandler"></param>
/// <param name="forgotPasswordHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/scores")]
public class AuthorizationsController(
    ISignUpHandler signUpHandler,
    ISignInHandler signInHandler,
    IChangePasswordHandler changePasswordHandler,
    IForgotPasswordHandler forgotPasswordHandler) : BaseApiController
{
    /// <summary>
    /// Handles user sign-up requests by invoking the sign-up handler and returning the appropriate response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("sign_up")]
    [ProducesResponseType<Service200Response<object>>(statusCode: StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        return ReturnResponseWithHttpStatus(await signUpHandler.Handler(request));
    }

    /// <summary>
    /// Handles user sign-in requests by invoking the sign-in handler and returning the appropriate response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("sign_in")]
    [ProducesResponseType<Service200Response<object>>(statusCode: StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        return ReturnResponseWithHttpStatus(await signInHandler.Handler(request));
    }

    /// <summary>
    /// Handles user password change requests by invoking the change password handler and returning the appropriate response. This endpoint requires authorization.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("change_password")]
    [ProducesResponseType<Service200Response<object>>(statusCode: StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        return ReturnResponseWithHttpStatus(await changePasswordHandler.Handler(request));
    }

    /// <summary>
    /// Handles user password recovery requests by invoking the forgot password handler and returning the appropriate response.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("forgot_password")]
    [ProducesResponseType<Service200Response<object>>(statusCode: StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
    {
        return ReturnResponseWithHttpStatus(await forgotPasswordHandler.Handler(request));
    }
}