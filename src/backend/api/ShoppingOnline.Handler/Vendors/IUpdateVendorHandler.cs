using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Service.Vendors;

namespace ShoppingOnline.Handler.Vendors;

public interface IUpdateVendorHandler : IBaseHandler<UpdateVendorRequest, ServiceResponse>;

public class UpdateVendorHandler(ILogger logger, IVendorService service)
    : BaseHandler<IVendorService, UpdateVendorRequest, ServiceResponse>(logger, service), IUpdateVendorHandler
{
    public override async Task<ServiceResponse> Handler(UpdateVendorRequest request)
        => await service.UpdateVendorAsync(request);
}
