using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IGetProfileHandler : IBaseHandler<GetProfileRequest, ServiceResponse>;

public class GetProfileHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, GetProfileRequest, ServiceResponse>(logger, service), IGetProfileHandler
{
    public override async Task<ServiceResponse> Handler(GetProfileRequest request)
        => await service.GetProfileAsync(request);
}
