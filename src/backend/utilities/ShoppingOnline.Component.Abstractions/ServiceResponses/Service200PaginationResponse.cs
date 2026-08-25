namespace ShoppingOnline.Component.Abstractions.ServiceResponses;

/// <summary>
/// Service response with Http Status 200 OK and pagination
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data"></param>
/// <param name="pageIndex"></param>
/// <param name="pageLimit"></param>
/// <param name="totalRecords"></param>
/// <param name="totalPages"></param>
/// <param name="isSuccess"></param>
[JsonObject]
public class Service200PaginationResponse<T>(
    T data,
    int pageIndex,
    int pageLimit,
    int totalRecords,
    int totalPages,
    bool isSuccess = true) : Service200Response<T>(data, isSuccess)
{
    /// <summary>
    /// Page Index
    /// </summary>
    [JsonProperty("page_index")]
    public int PageIndex { get; set; } = pageIndex;

    /// <summary>
    /// Page limit
    /// </summary>
    [JsonProperty("page_limit")]
    public int PageLimit { get; set; } = pageLimit;

    /// <summary>
    /// Total record
    /// </summary>
    [JsonProperty("total_records")]
    public int TotalRecords { get; set; } = totalRecords;

    /// <summary>
    /// Total pages
    /// </summary>
    [JsonProperty("total_pages")]
    public int TotalPages { get; set; } = totalPages;
}

/// <summary>
/// Success response with Http Status 200 Ok and pagination
/// </summary>
/// <param name="data"></param>
/// <param name="pageIndex"></param>
/// <param name="pageLimit"></param>
/// <param name="totalRecords"></param>
/// <param name="totalPages"></param>
public class Service200PaginationResponse(object data, int pageIndex, int pageLimit, int totalRecords, int totalPages)
    : Service200PaginationResponse<object>(data, pageIndex, pageLimit, totalRecords, totalPages);