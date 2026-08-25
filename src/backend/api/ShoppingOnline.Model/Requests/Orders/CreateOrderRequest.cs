namespace ShoppingOnline.Model.Requests.Orders;

public class CreateOrderRequest
{
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}
