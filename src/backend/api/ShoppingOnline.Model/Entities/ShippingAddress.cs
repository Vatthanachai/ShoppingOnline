using ShoppingOnline.Component.Abstractions.Models;

namespace ShoppingOnline.Model.Entities;

public class ShippingAddress : IAuditable
{
    public int ShippingAddressId { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
}