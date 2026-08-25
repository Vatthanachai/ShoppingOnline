namespace ShoppingOnline.Model.Requests.Vendors;

public class UpdateVendorRequest
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
