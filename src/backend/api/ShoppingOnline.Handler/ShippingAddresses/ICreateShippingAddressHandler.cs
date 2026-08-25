using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface ICreateShippingAddressHandler : IBaseHandler<CreateShippingAddressRequest, ServiceResponse>;

public class CreateShippingAddressHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, CreateShippingAddressRequest, ServiceResponse>(logger, service),
        ICreateShippingAddressHandler
{
    public override async Task<ServiceResponse> Handler(CreateShippingAddressRequest request)
        => await service.CreateShippingAddressAsync(request);
}
