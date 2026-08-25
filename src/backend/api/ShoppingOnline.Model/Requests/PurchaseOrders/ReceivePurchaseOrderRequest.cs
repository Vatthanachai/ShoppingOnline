namespace ShoppingOnline.Model.Requests.PurchaseOrders;

public class ReceivePurchaseOrderLineRequest
{
    public int PurchaseOrderItemId { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitCost { get; set; }
}

public class ReceivePurchaseOrderRequest
{
    public int PurchaseOrderId { get; set; }
    public List<ReceivePurchaseOrderLineRequest> Lines { get; set; } = [];
}
