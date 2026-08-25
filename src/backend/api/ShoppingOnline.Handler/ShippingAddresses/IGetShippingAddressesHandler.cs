using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface IGetShippingAddressesHandler : IBaseHandler<GetShippingAddressesRequest, ServiceResponse>;

public class GetShippingAddressesHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, GetShippingAddressesRequest, ServiceResponse>(logger, service),
        IGetShippingAddressesHandler
{
    public override async Task<ServiceResponse> Handler(GetShippingAddressesRequest request)
        => await service.GetShippingAddressesAsync(request);
}
