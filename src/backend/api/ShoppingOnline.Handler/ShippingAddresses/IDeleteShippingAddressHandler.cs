using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Service.ShippingAddresses;

namespace ShoppingOnline.Handler.ShippingAddresses;

public interface IDeleteShippingAddressHandler : IBaseHandler<DeleteShippingAddressRequest, ServiceResponse>;

public class DeleteShippingAddressHandler(ILogger logger, IShippingAddressService service)
    : BaseHandler<IShippingAddressService, DeleteShippingAddressRequest, ServiceResponse>(logger, service),
        IDeleteShippingAddressHandler
{
    public override async Task<ServiceResponse> Handler(DeleteShippingAddressRequest request)
        => await service.DeleteShippingAddressAsync(request);
}
