using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface IUpdateShippingAddressHandler : IBaseHandler<UpdateShippingAddressRequest, ServiceResponse>;

public class UpdateShippingAddressHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, UpdateShippingAddressRequest, ServiceResponse>(logger, service),
        IUpdateShippingAddressHandler
{
    public override async Task<ServiceResponse> Handler(UpdateShippingAddressRequest request)
        => await service.UpdateShippingAddressAsync(request);
}
