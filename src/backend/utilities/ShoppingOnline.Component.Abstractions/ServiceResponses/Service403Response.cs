namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service Response with Http Status 403 Forbidden
/// </summary>
/// <param name="message"></param>
[Serializable, JsonObject]
public class Service403Response(string message = null) : ServiceResponse
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = message;
}