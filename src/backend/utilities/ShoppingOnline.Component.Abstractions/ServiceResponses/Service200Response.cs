namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Represents a successful service response that includes a result of the specified type.
/// </summary>
/// <typeparam name="T">The type of the data returned in the response.</typeparam>
/// <param name="data">The data to include in the response. This value is assigned to the <see cref="Data"/> property.</param>
/// <param name="isSuccess">A value indicating whether the operation was successful. Defaults to <see langword="true"/>.</param>
[Serializable, JsonObject]
public class Service200Response<T>(T data, bool isSuccess = true) : ServiceResponse
{
    /// <summary>
    /// Process status
    /// </summary>
    [JsonProperty("is_success")]
    public bool IsSuccess { get; set; } = isSuccess;

    /// <summary>
    /// Data
    /// </summary>
    [JsonProperty]
    public T Data { get; set; } = data;
}

/// <summary>
/// Represents a successful service response containing arbitrary data.
/// </summary>
/// <param name="data">The data to include in the response. Can be any object representing the result of the service operation.</param>
public class Service200Response(object data) : Service200Response<object>(data);