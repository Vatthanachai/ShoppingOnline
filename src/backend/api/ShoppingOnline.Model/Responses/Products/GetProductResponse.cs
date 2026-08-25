namespace ShoppingOnline.Model.Responses.Products;

public class GetProductResponse
{
    public int ProductId { get; set; }
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public decimal? MinPrice { get; set; }
    public string? ImagePath { get; set; }
}
