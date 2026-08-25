using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.Controllers;
using ShoppingOnline.Handler.ProductCategories;
using ShoppingOnline.Model.Requests.Categories;

namespace ShoppingOnline.API.Controllers;

/// <summary>
/// Controller for managing product categories in the ShoppingOnline application.
/// </summary>
/// <param name="getCategoriesHandler"></param>
/// <param name="getCategoryHandler"></param>
/// <param name="createCategoryHandler"></param>
/// <param name="updateCategoryHandler"></param>
/// <param name="deactivateCategoryHandler"></param>
[ApiController, ApiVersion("1.0"), Route("api/product_categories")]
public class ProductCategoriesController(
    IGetCategoriesHandler getCategoriesHandler,
    IGetCategoryHandler getCategoryHandler,
    ICreateCategoryHandler createCategoryHandler,
    IUpdateCategoryHandler updateCategoryHandler,
    IDeactivateCategoryHandler deactivateCategoryHandler) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of product categories based on the specified request parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] GetCategoriesRequest request)
        => ReturnResponseWithHttpStatus(await getCategoriesHandler.Handler(request));

    /// <summary>
    /// Retrieves a specific product category by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryAsync(int id)
        => ReturnResponseWithHttpStatus(await getCategoryHandler.Handler(new GetCategoryRequest { CategoryId = id }));

    /// <summary>
    /// Creates a new product category based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
        => ReturnResponseWithHttpStatus(await createCategoryHandler.Handler(request));

    /// <summary>
    /// Updates an existing product category identified by its ID with the provided request data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryRequest request)
    {
        request.CategoryId = id;
        return ReturnResponseWithHttpStatus(await updateCategoryHandler.Handler(request));
    }

    /// <summary>
    /// Deactivates a product category identified by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateCategoryAsync(int id)
        => ReturnResponseWithHttpStatus(
            await deactivateCategoryHandler.Handler(new DeactivateCategoryRequest { CategoryId = id }));
}
