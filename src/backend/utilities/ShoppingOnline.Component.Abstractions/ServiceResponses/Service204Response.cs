namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Represents a response from a service that indicates a successful operation with no content returned (HTTP 204 No
/// Content).
/// </summary>
/// <remarks>Use this type to model service responses where the operation completes successfully but does not
/// return any data in the response body. This is commonly used for operations such as deletions or updates where no
/// additional information is required in the response.</remarks>
[Serializable, JsonObject]
public class Service204Response : ServiceResponse;