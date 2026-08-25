using Serilog;

using ShoppingOnline.Component.Abstractions.Handlers;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Service.Users;

namespace ShoppingOnline.Handler.Users;

public interface IDeactivateAccountHandler : IBaseHandler<DeactivateAccountRequest, ServiceResponse>;

public class DeactivateAccountHandler(ILogger logger, IUserService service)
    : BaseHandler<IUserService, DeactivateAccountRequest, ServiceResponse>(logger, service),
        IDeactivateAccountHandler
{
    public override async Task<ServiceResponse> Handler(DeactivateAccountRequest request)
        => await service.DeactivateAccountAsync(request);
}
