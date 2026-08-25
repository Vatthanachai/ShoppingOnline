namespace ShoppingOnline.Model.Requests.Stocks;

public class CreateStockRequest
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
