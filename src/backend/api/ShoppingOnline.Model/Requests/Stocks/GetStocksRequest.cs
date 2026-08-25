using ShoppingOnline.Component.Abstractions.Requests;

namespace ShoppingOnline.Model.Requests.Stocks;

public class GetStocksRequest : PagingRequest
{
    public int? ProductId { get; set; }
    public int? VendorId { get; set; }
}
