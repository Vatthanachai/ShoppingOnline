using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class ProductCategory : IAuditable, IActive
{
    public int ProductCategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
}