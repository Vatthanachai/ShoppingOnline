namespace ShoppingOnline.Model.Responses.Orders;

public class OrderItemResponse
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
