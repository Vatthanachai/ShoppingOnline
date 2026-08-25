using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

/// <summary>
/// A single received lot of inventory for a (Product, Vendor) pair. Orders are fulfilled
/// FIFO across lots by <see cref="IAuditable.CreatedOn"/> (oldest lot first), regardless of
/// which vendor supplied it - see OrderService.CreateOrderAsync.
/// </summary>
public class Stock : IAuditable
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public int Quantity { get; set; }

    /// <summary>Unit cost paid to the vendor for this lot - not the customer-facing sell price (see Product.SellPrice).</summary>
    public decimal Cost { get; set; }

    /// <summary>The PO line this lot was received against, if any (seed/manually-entered lots have none).</summary>
    public int? PurchaseOrderItemId { get; set; }
    public virtual PurchaseOrderItem? PurchaseOrderItem { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
