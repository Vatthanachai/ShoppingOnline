using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Service.Vendors;

namespace ShoppingOnline.Handler.Vendors;

public interface IDeactivateVendorHandler : IBaseHandler<DeactivateVendorRequest, ServiceResponse>;

public class DeactivateVendorHandler(ILogger logger, IVendorService service)
    : BaseHandler<IVendorService, DeactivateVendorRequest, ServiceResponse>(logger, service), IDeactivateVendorHandler
{
    public override async Task<ServiceResponse> Handler(DeactivateVendorRequest request)
        => await service.DeactivateVendorAsync(request);
}
