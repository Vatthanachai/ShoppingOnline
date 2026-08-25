namespace ShoppingOnline.Model.Responses.Vendors;

public class GetVendorsResponse
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
}
