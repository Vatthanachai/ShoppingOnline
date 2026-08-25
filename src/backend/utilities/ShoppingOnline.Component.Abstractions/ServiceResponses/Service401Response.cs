namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service Response with Http Status 401 Un Authorize
/// </summary>
/// <param name="message"></param>
[Serializable, JsonObject]
public class Service401Response(string message = null) : ServiceResponse
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = message;
}