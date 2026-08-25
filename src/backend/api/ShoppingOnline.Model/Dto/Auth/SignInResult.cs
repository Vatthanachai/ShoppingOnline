namespace ShoppingOnline.Model.Dto.Auth;

public class SignInResult
{
    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool MustChangePassword { get; set; }
}
