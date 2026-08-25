namespace ShoppingOnline.Model.Entities;

/// <summary>
/// One FIFO slice of an OrderItem: how many units came out of a specific Stock lot (and
/// therefore which vendor supplied them). An OrderItem can have several of these when a
/// single line's quantity had to be filled from more than one lot.
/// </summary>
public class OrderItemAllocation
{
    public int OrderItemAllocationId { get; set; }
    public int OrderItemId { get; set; }
    public virtual OrderItem OrderItem { get; set; }
    public int StockId { get; set; }
    public virtual Stock Stock { get; set; }
    public int VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }
    public int Quantity { get; set; }
}
