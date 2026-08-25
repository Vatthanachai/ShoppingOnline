namespace ShoppingOnline.Model.Requests.PurchaseOrders;

public class CreatePurchaseOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCostQuoted { get; set; }
}

public class CreatePurchaseOrderRequest
{
    public int VendorId { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = [];
}
