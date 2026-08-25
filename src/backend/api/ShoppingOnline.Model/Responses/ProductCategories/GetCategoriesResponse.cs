namespace ShoppingOnline.Model.Responses.Categories;

public class GetCategoriesResponse
{
    public int ProductCategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}