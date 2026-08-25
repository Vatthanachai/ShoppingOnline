namespace ShoppingOnline.Model.Requests.Categories;

public class UpdateCategoryRequest
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
}
