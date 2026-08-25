using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Service.Vendors;

namespace ShoppingOnline.Handler.Vendors;

public interface IGetVendorsHandler : IBaseHandler<GetVendorsRequest, ServiceResponse>;

public class GetVendorsHandler(ILogger logger, IVendorService service)
    : BaseHandler<IVendorService, GetVendorsRequest, ServiceResponse>(logger, service), IGetVendorsHandler
{
    public override async Task<ServiceResponse> Handler(GetVendorsRequest request)
        => await service.GetVendorsAsync(request);
}
