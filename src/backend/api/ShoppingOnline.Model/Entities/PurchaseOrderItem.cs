namespace ShoppingOnline.Model.Entities;

public class PurchaseOrderItem
{
    public int PurchaseOrderItemId { get; set; }
    public int PurchaseOrderId { get; set; }
    public virtual PurchaseOrder PurchaseOrder { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }

    public int QuantityOrdered { get; set; }

    /// <summary>Running total received across all receiving events for this line.</summary>
    public int QuantityReceived { get; set; }

    /// <summary>Optional reference cost shown on the PO document sent to the vendor - the actual cost recorded on each received lot is entered fresh at receiving time.</summary>
    public decimal? UnitCostQuoted { get; set; }

    public virtual ICollection<Stock> ReceivedLots { get; set; } = new HashSet<Stock>();
}
