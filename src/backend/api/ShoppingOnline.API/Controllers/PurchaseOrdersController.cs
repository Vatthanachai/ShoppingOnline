using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.PurchaseOrders;
using ShoppingOnline.Model.Requests.PurchaseOrders;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing purchase orders (restocking a product via a vendor) in the
/// ShoppingOnline application. Admin-only: this is how new Stock lots get created.
/// </summary>
/// <param name="getPurchaseOrdersHandler"></param>
/// <param name="getPurchaseOrderHandler"></param>
/// <param name="createPurchaseOrderHandler"></param>
/// <param name="sendPurchaseOrderHandler"></param>
/// <param name="receivePurchaseOrderHandler"></param>
[Authorize(Roles = "Admin"), ApiController, ApiVersion("1.0"), Route("api/purchase_orders")]
public class PurchaseOrdersController(
    IGetPurchaseOrdersHandler getPurchaseOrdersHandler,
    IGetPurchaseOrderHandler getPurchaseOrderHandler,
    ICreatePurchaseOrderHandler createPurchaseOrderHandler,
    ISendPurchaseOrderHandler sendPurchaseOrderHandler,
    IReceivePurchaseOrderHandler receivePurchaseOrderHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of purchase orders based on the specified request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetPurchaseOrdersAsync([FromQuery] GetPurchaseOrdersRequest request)
        => ReturnResponseWithHttpStatus(await getPurchaseOrdersHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific purchase order by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPurchaseOrderAsync(int id)
        => ReturnResponseWithHttpStatus(await getPurchaseOrderHandler.Handler(new GetPurchaseOrderRequest { PurchaseOrderId = id }));

    /// <summary>
    /// Creates a new draft purchase order.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreatePurchaseOrderAsync([FromBody] CreatePurchaseOrderRequest request)
        => ReturnResponseWithHttpStatus(await createPurchaseOrderHandler.Handler(request));

    /// <summary>
    /// Sends a draft purchase order to its vendor by email.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id:int}/send")]
    public async Task<IActionResult> SendPurchaseOrderAsync(int id)
        => ReturnResponseWithHttpStatus(await sendPurchaseOrderHandler.Handler(new SendPurchaseOrderRequest { PurchaseOrderId = id }));

    /// <summary>
    /// Records stock received against a sent purchase order (supports partial receipt across
    /// multiple calls) - each line creates a new FIFO stock lot.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id:int}/receive")]
    public async Task<IActionResult> ReceivePurchaseOrderAsync(int id, [FromBody] ReceivePurchaseOrderRequest request)
    {
        request.PurchaseOrderId = id;
        return ReturnResponseWithHttpStatus(await receivePurchaseOrderHandler.Handler(request));
    }
}
