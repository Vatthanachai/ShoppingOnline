using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class Vendor : IAuditable, IActive
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public virtual ICollection<Stock> Stocks { get; set; } = new HashSet<Stock>();
}