namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service Response with Http Status 404 Not found
/// </summary>
/// <param name="message"></param>
[Serializable, JsonObject]
public class Service404Response(string message = null) : ServiceResponse
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = message;
}