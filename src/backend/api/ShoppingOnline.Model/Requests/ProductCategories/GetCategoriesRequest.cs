using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Categories;

public class GetCategoriesRequest : PagingRequest
{
    public string? Search { get; set; }
    public bool IncludeInactive { get; set; }
}