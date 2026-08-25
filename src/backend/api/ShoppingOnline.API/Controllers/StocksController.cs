using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Stocks;
using ShoppingOnline.Model.Requests.Stocks;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing stocks in the ShoppingOnline application.
/// </summary>
/// <param name="getStocksHandler"></param>
/// <param name="getStockHandler"></param>
/// <param name="createStockHandler"></param>
/// <param name="updateStockHandler"></param>
/// <param name="deleteStockHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/stocks")]
public class StocksController(
    IGetStocksHandler getStocksHandler,
    IGetStockHandler getStockHandler,
    ICreateStockHandler createStockHandler,
    IUpdateStockHandler updateStockHandler,
    IDeleteStockHandler deleteStockHandler) : BaseApiController
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

    /// <summary>
    /// Creates a new stock based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateStockAsync([FromBody] CreateStockRequest request)
        => ReturnResponseWithHttpStatus(await createStockHandler.Handler(request));

    /// <summary>
    /// Updates an existing stock identified by its ID with the provided request data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStockAsync(int id, [FromBody] UpdateStockRequest request)
    {
        request.StockId = id;
        return ReturnResponseWithHttpStatus(await updateStockHandler.Handler(request));
    }

    /// <summary>
    /// Deletes a specific stock identified by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStockAsync(int id)
        => ReturnResponseWithHttpStatus(await deleteStockHandler.Handler(new DeleteStockRequest { StockId = id }));
}
