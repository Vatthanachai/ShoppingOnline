using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Stocks;
using ShoppingOnline.Model.Requests.Stocks;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Read-only inventory lookup. Stock lots are created only via a received Purchase Order
/// (see PurchaseOrdersController) and consumed FIFO by order creation - there is no
/// create/update/delete here.
/// </summary>
/// <param name="getStocksHandler"></param>
/// <param name="getStockHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/stocks")]
public class StocksController(
    IGetStocksHandler getStocksHandler,
    IGetStockHandler getStockHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of stocks based on the provided request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetStocksAsync([FromQuery] GetStocksRequest request)
        => ReturnResponseWithHttpStatus(await getStocksHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific stock by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStockAsync(int id)
        => ReturnResponseWithHttpStatus(await getStockHandler.Handler(new GetStockRequest { StockId = id }));
}
