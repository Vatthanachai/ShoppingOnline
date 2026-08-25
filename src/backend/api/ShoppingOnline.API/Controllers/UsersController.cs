using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Users;
using ShoppingOnline.Model.Requests.Users;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing users in the ShoppingOnline application.
/// </summary>
/// <param name="getUsersHandler"></param>
/// <param name="getUserHandler"></param>
/// <param name="activateUserHandler"></param>
/// <param name="deactivateUserHandler"></param>
[Authorize(Roles = "Admin"), ApiController, ApiVersion("1.0"), Route("api/users")]
public class UsersController(
    IGetUsersHandler getUsersHandler,
    IGetUserHandler getUserHandler,
    IActivateUserHandler activateUserHandler,
    IDeactivateUserHandler deactivateUserHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of users based on the provided request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetUsersRequest request)
        => ReturnResponseWithHttpStatus(await getUsersHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserAsync(int id)
        => ReturnResponseWithHttpStatus(await getUserHandler.Handler(new GetUserRequest { UserId = id }));

    /// <summary>
    /// Activates a user account based on the provided user ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> ActivateUserAsync(int id)
        => ReturnResponseWithHttpStatus(await activateUserHandler.Handler(new ActivateUserRequest { UserId = id }));

    /// <summary>
    /// Deactivates a user account based on the provided user ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateUserAsync(int id)
        => ReturnResponseWithHttpStatus(
            await deactivateUserHandler.Handler(new DeactivateUserRequest { UserId = id }));
}
