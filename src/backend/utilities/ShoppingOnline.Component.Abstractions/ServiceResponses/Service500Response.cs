namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service Response with Http Status 500 Internal Server Error
/// </summary>
/// <param name="exception"></param>
[Serializable, JsonObject]
public class Service500Response(Exception exception) : ServiceResponse
{
    /// <summary>
    /// Exception
    /// </summary>
    public Exception Exception { get; set; } = exception;
}