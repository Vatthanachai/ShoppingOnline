using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Model.Responses.Orders;

public class GetOrderResponse
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    public string ShippingAddressLine1 { get; set; }
    public string ShippingAddressLine2 { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
    public string ShippingPostalCode { get; set; }
    public string ShippingCountry { get; set; }

    public List<OrderItemResponse> Items { get; set; } = [];
}
