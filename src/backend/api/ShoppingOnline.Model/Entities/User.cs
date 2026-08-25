using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class User : IAuditable, IActive
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string PasswordHash { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public virtual ICollection<ShippingAddress> ShippingAddresses { get; set; } = new HashSet<ShippingAddress>();
}