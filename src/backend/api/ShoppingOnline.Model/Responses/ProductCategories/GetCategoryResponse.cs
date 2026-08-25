namespace ShoppingOnline.Model.Responses.Categories;

public class GetCategoryResponse
{
    public int ProductCategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}