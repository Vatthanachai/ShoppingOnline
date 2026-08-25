using ShoppingOnline.Component.Abstractions.Requests;
using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Model.Requests.PurchaseOrders;

public class GetPurchaseOrdersRequest : PagingRequest
{
    public int? VendorId { get; set; }
    public PurchaseOrderStatus? Status { get; set; }
}
