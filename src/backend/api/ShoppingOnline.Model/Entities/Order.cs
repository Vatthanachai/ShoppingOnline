using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class Order : IAuditable
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}