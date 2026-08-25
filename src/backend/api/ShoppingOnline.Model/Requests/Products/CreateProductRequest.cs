namespace ShoppingOnline.Model.Requests.Products;

public class CreateProductRequest
{
    public int ProductCategoryId { get; set; }
    public int VendorId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public string? ImagePath { get; set; }
}
