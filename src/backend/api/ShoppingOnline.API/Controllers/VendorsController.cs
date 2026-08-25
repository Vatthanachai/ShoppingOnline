using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Vendors;
using ShoppingOnline.Model.Requests.Vendors;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing vendors in the ShoppingOnline application.
/// </summary>
/// <param name="getVendorsHandler"></param>
/// <param name="getVendorHandler"></param>
/// <param name="createVendorHandler"></param>
/// <param name="updateVendorHandler"></param>
/// <param name="deactivateVendorHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/vendors")]
public class VendorsController(
    IGetVendorsHandler getVendorsHandler,
    IGetVendorHandler getVendorHandler,
    ICreateVendorHandler createVendorHandler,
    IUpdateVendorHandler updateVendorHandler,
    IDeactivateVendorHandler deactivateVendorHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of vendors based on the provided request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetVendorsAsync([FromQuery] GetVendorsRequest request)
        => ReturnResponseWithHttpStatus(await getVendorsHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific vendor by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVendorAsync(int id)
        => ReturnResponseWithHttpStatus(await getVendorHandler.Handler(new GetVendorRequest { VendorId = id }));

    /// <summary>
    /// Creates a new vendor based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateVendorAsync([FromBody] CreateVendorRequest request)
        => ReturnResponseWithHttpStatus(await createVendorHandler.Handler(request));

    /// <summary>
    /// Updates an existing vendor identified by its ID with the provided request data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVendorAsync(int id, [FromBody] UpdateVendorRequest request)
    {
        request.VendorId = id;
        return ReturnResponseWithHttpStatus(await updateVendorHandler.Handler(request));
    }

    /// <summary>
    /// Deactivates a vendor identified by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateVendorAsync(int id)
        => ReturnResponseWithHttpStatus(
            await deactivateVendorHandler.Handler(new DeactivateVendorRequest { VendorId = id }));
}
