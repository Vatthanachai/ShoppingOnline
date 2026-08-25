using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Users;

namespace ShoppingOnline.Service.Users;

public interface IUserService : IBaseService<User>
{
    Task<ServiceResponse> GetProfileAsync(GetProfileRequest request);
    Task<ServiceResponse> UpdateProfileAsync(UpdateProfileRequest request);
    Task<ServiceResponse> DeactivateAccountAsync(DeactivateAccountRequest request);

    Task<ServiceResponse> GetUsersAsync(GetUsersRequest request);
    Task<ServiceResponse> GetUserAsync(GetUserRequest request);
    Task<ServiceResponse> ActivateUserAsync(ActivateUserRequest request);
    Task<ServiceResponse> DeactivateUserAsync(DeactivateUserRequest request);
}
