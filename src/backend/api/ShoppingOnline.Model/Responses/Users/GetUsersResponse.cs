using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Model.Responses.Users;

public class GetUsersResponse
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}
