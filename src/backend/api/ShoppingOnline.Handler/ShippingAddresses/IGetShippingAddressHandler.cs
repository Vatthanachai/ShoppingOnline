using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface IGetShippingAddressHandler : IBaseHandler<GetShippingAddressRequest, ServiceResponse>;

public class GetShippingAddressHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, GetShippingAddressRequest, ServiceResponse>(logger, service),
        IGetShippingAddressHandler
{
    public override async Task<ServiceResponse> Handler(GetShippingAddressRequest request)
        => await service.GetShippingAddressAsync(request);
}
