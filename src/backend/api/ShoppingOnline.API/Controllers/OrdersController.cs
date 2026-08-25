using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Orders;
using ShoppingOnline.Model.Requests.Orders;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing orders in the ShoppingOnline application.
/// </summary>
/// <param name="getOrdersHandler"></param>
/// <param name="getOrderHandler"></param>
/// <param name="createOrderHandler"></param>
/// <param name="cancelOrderHandler"></param>
[Authorize, ApiController, ApiVersion("1.0"), Route("api/orders")]
public class OrdersController(
    IGetOrdersHandler getOrdersHandler,
    IGetOrderHandler getOrderHandler,
    ICreateOrderHandler createOrderHandler,
    ICancelOrderHandler cancelOrderHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of orders based on the specified request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] GetOrdersRequest request)
        => ReturnResponseWithHttpStatus(await getOrdersHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderAsync(int id)
        => ReturnResponseWithHttpStatus(await getOrderHandler.Handler(new GetOrderRequest { OrderId = id }));

    /// <summary>
    /// Creates a new order based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderRequest request)
        => ReturnResponseWithHttpStatus(await createOrderHandler.Handler(request));

    /// <summary>
    /// Cancels an existing order by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrderAsync(int id)
        => ReturnResponseWithHttpStatus(await cancelOrderHandler.Handler(new CancelOrderRequest { OrderId = id }));
}