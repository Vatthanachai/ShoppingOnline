using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Service.Vendors;

namespace ShoppingOnline.Handler.Vendors;

public interface IGetVendorHandler : IBaseHandler<GetVendorRequest, ServiceResponse>;

public class GetVendorHandler(ILogger logger, IVendorService service)
    : BaseHandler<IVendorService, GetVendorRequest, ServiceResponse>(logger, service), IGetVendorHandler
{
    public override async Task<ServiceResponse> Handler(GetVendorRequest request)
        => await service.GetVendorAsync(request);
}
