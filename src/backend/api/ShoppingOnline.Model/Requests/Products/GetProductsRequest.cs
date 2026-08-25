using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Products;

public class GetProductsRequest : PagingRequest
{
    public string? Search { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? VendorId { get; set; }
}
