using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class Order : IAuditable
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    // Snapshot of the shipping address picked at checkout - copied from ShippingAddress
    // rather than a live FK, so editing/deleting a saved address later never changes or
    // breaks a past order's recorded destination.
    public string ShippingAddressLine1 { get; set; }
    public string ShippingAddressLine2 { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
    public string ShippingPostalCode { get; set; }
    public string ShippingCountry { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}
