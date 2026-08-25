namespace ShoppingOnline.Model.Responses.PurchaseOrders;

public class PurchaseOrderItemResponse
{
    public int PurchaseOrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public decimal? UnitCostQuoted { get; set; }
}
