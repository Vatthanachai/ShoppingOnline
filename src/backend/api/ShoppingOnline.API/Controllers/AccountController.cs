using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Users;
using ShoppingOnline.Model.Requests.Users;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing user account-related operations, including profile retrieval, profile updates, and account deactivation.
/// </summary>
/// <param name="getProfileHandler"></param>
/// <param name="updateProfileHandler"></param>
/// <param name="deactivateAccountHandler"></param>
[Authorize, ApiController, ApiVersion("1.0"), Route("api/account")]
public class AccountController(
    IGetProfileHandler getProfileHandler,
    IUpdateProfileHandler updateProfileHandler,
    IDeactivateAccountHandler deactivateAccountHandler) : BaseApiController
{
    /// <summary>
    /// Get the profile of the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync()
        => ReturnResponseWithHttpStatus(await getProfileHandler.Handler(new GetProfileRequest()));

    /// <summary>
    /// Update the profile of the currently authenticated user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileRequest request)
        => ReturnResponseWithHttpStatus(await updateProfileHandler.Handler(request));

    /// <summary>
    /// Deactivate the account of the currently authenticated user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("deactivate")]
    public async Task<IActionResult> DeactivateAccountAsync([FromBody] DeactivateAccountRequest request)
        => ReturnResponseWithHttpStatus(await deactivateAccountHandler.Handler(request));
}