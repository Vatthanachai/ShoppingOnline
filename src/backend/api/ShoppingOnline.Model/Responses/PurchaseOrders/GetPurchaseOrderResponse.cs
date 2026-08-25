using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Model.Responses.PurchaseOrders;

public class GetPurchaseOrderResponse
{
    public int PurchaseOrderId { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string VendorEmail { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? SentOn { get; set; }
    public List<PurchaseOrderItemResponse> Items { get; set; } = [];
}
