using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class OrderItem : IAuditable
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}