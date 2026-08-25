namespace ShoppingOnline.Model.Requests.Stocks;

public class UpdateStockRequest
{
    public int StockId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
