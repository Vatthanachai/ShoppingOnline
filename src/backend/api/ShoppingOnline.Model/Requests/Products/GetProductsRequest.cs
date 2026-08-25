using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Products;

public class GetProductsRequest : PagingRequest
{
    public string? Search { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? VendorId { get; set; }

    /// <summary>When false (default), only active products are returned - the public storefront's behavior. The admin UI passes true to manage every product regardless of status.</summary>
    public bool IncludeInactive { get; set; }
}
