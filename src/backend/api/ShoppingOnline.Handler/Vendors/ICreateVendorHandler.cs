using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Service.Vendors;

namespace ShoppingOnline.Handler.Vendors;

public interface ICreateVendorHandler : IBaseHandler<CreateVendorRequest, ServiceResponse>;

public class CreateVendorHandler(ILogger logger, IVendorService service)
    : BaseHandler<IVendorService, CreateVendorRequest, ServiceResponse>(logger, service), ICreateVendorHandler
{
    public override async Task<ServiceResponse> Handler(CreateVendorRequest request)
        => await service.CreateVendorAsync(request);
}
