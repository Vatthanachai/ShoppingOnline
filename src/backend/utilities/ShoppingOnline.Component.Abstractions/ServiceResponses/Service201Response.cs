namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Represents a response returned by the service for a successful resource creation (HTTP 201 Created).
/// </summary>
/// <remarks>This class is typically used to encapsulate information about a newly created resource, including any
/// relevant metadata or identifiers. It inherits from <see cref="ServiceResponse"/>, which may provide additional
/// response details such as status codes or headers.</remarks>
/// <param name="message">Optional message to include with the successful response</param>
[Serializable, JsonObject]
public class Service201Response(string message = null) : ServiceResponse
{
    /// <summary>
    /// Optional message for the response
    /// </summary>
    public string Message { get; set; } = message;
}

/// <summary>
/// Represents a response indicating that a service request has been accepted for processing but is not yet complete
/// (HTTP 202 Accepted).
/// </summary>
/// <remarks>Use this response type to communicate that the request has been received and understood, and that
/// processing will occur asynchronously. The client may need to poll or await further notification to determine the
/// final outcome of the request.</remarks>
[Serializable, JsonObject]
public class Service202Response : ServiceResponse;