namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service Response with Http Status 409 Conflict
/// </summary>
/// <param name="message"></param>
public class Service409Response(string message = null) : ServiceResponse
{
    public string Message { get; set; } = message;
}