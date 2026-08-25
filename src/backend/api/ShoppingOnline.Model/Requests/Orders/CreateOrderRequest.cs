namespace ShoppingOnline.Model.Requests.Orders;

public class CreateOrderRequest
{
    public int ShippingAddressId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}
