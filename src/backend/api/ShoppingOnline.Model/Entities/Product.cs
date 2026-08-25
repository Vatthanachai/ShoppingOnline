using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class Product : IAuditable, IActive
{
    public int ProductId { get; set; }
    public int ProductCategoryId { get; set; }
    public virtual ProductCategory ProductCategory { get; set; }
    public int VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = new HashSet<Stock>();
}