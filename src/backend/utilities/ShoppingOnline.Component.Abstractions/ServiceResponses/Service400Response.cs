namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Represents a response indicating a bad request (HTTP 400) from a service, including an optional error message.
/// </summary>
/// <param name="message">The error message that describes the reason for the bad request. Can be null if no message is provided.</param>
[Serializable, JsonObject]
public class Service400Response(string message = null) : ServiceResponse
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = message;
}