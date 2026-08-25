using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Model.Responses.Orders;

public class GetOrderResponse
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemResponse> Items { get; set; } = [];
}
