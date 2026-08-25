using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.ShippingAddresses;
using ShoppingOnline.Model.Requests.ShippingAddresses;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing shipping addresses in the ShoppingOnline application.
/// </summary>
/// <param name="getShippingAddressesHandler"></param>
/// <param name="getShippingAddressHandler"></param>
/// <param name="createShippingAddressHandler"></param>
/// <param name="updateShippingAddressHandler"></param>
/// <param name="deleteShippingAddressHandler"></param>
[Authorize, ApiController, ApiVersion("1.0"), Route("api/shipping_addresses")]
public class ShippingAddressesController(
    IGetShippingAddressesHandler getShippingAddressesHandler,
    IGetShippingAddressHandler getShippingAddressHandler,
    ICreateShippingAddressHandler createShippingAddressHandler,
    IUpdateShippingAddressHandler updateShippingAddressHandler,
    IDeleteShippingAddressHandler deleteShippingAddressHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of shipping addresses based on the provided request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetShippingAddressesAsync([FromQuery] GetShippingAddressesRequest request)
        => ReturnResponseWithHttpStatus(await getShippingAddressesHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific shipping address by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShippingAddressAsync(int id)
        => ReturnResponseWithHttpStatus(
            await getShippingAddressHandler.Handler(new GetShippingAddressRequest { ShippingAddressId = id }));

    /// <summary>
    /// Creates a new shipping address based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateShippingAddressAsync([FromBody] CreateShippingAddressRequest request)
        => ReturnResponseWithHttpStatus(await createShippingAddressHandler.Handler(request));

    /// <summary>
    /// Updates an existing shipping address identified by its ID with the provided request data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateShippingAddressAsync(int id, [FromBody] UpdateShippingAddressRequest request)
    {
        request.ShippingAddressId = id;
        return ReturnResponseWithHttpStatus(await updateShippingAddressHandler.Handler(request));
    }

    /// <summary>
    /// Deletes a specific shipping address identified by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteShippingAddressAsync(int id)
        => ReturnResponseWithHttpStatus(
            await deleteShippingAddressHandler.Handler(new DeleteShippingAddressRequest { ShippingAddressId = id }));
}
