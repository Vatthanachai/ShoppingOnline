namespace ShoppingOnline.Model.Responses.Stocks;

public class GetStockResponse
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
