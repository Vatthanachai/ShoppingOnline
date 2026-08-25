using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface ISetDefaultShippingAddressHandler : IBaseHandler<SetDefaultShippingAddressRequest, ServiceResponse>;

public class SetDefaultShippingAddressHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, SetDefaultShippingAddressRequest, ServiceResponse>(logger, service),
        ISetDefaultShippingAddressHandler
{
    public override async Task<ServiceResponse> Handler(SetDefaultShippingAddressRequest request)
        => await service.SetDefaultShippingAddressAsync(request);
}
