using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Users;

public class GetUsersRequest : PagingRequest
{
    public string? Search { get; set; }
}
