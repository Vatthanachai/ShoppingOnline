using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Vendors;

public class GetVendorsRequest : PagingRequest
{
    public string? Search { get; set; }
}
