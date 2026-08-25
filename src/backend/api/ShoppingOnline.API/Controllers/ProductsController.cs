using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.Products;
using ShoppingOnline.Model.Requests.Products;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing products in the ShoppingOnline application.
/// </summary>
/// <param name="getProductsHandler"></param>
/// <param name="getProductHandler"></param>
/// <param name="createProductHandler"></param>
/// <param name="updateProductHandler"></param>
/// <param name="deactivateProductHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/products")]
public class ProductsController(
    IGetProductsHandler getProductsHandler,
    IGetProductHandler getProductHandler,
    ICreateProductHandler createProductHandler,
    IUpdateProductHandler updateProductHandler,
    IDeactivateProductHandler deactivateProductHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of products based on the specified request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsRequest request)
        => ReturnResponseWithHttpStatus(await getProductsHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id)
        => ReturnResponseWithHttpStatus(await getProductHandler.Handler(new GetProductRequest { ProductId = id }));

    /// <summary>
    /// Creates a new product based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequest request)
        => ReturnResponseWithHttpStatus(await createProductHandler.Handler(request));

    /// <summary>
    /// Updates an existing product identified by its ID with the provided request data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductRequest request)
    {
        request.ProductId = id;
        return ReturnResponseWithHttpStatus(await updateProductHandler.Handler(request));
    }

    /// <summary>
    /// Deactivates a product identified by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateProductAsync(int id)
        => ReturnResponseWithHttpStatus(
            await deactivateProductHandler.Handler(new DeactivateProductRequest { ProductId = id }));
}
