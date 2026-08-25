using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class OrderItem : IAuditable
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int Quantity { get; set; }

    // Snapshot of Product.SellPrice/TaxRatePercent at order time - a customer's price never
    // changes retroactively if the admin edits the product later.
    public decimal UnitPrice { get; set; }
    public decimal TaxRatePercent { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Which stock lot(s)/vendor(s) FIFO-fulfilled this line - internal bookkeeping, not shown to the customer.</summary>
    public virtual ICollection<OrderItemAllocation> Allocations { get; set; } = new HashSet<OrderItemAllocation>();
}
