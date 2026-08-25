using Mapster;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.Securities;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Users;
using ShoppingOnline.Model.Responses.Users;

namespace ShoppingOnline.Service.Users;

public class UserService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor,
    IEncryptionService encryptionService)
    : BaseService<User, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IUserService
{
    public async Task<ServiceResponse> GetProfileAsync(GetProfileRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == userId.Value);
        if (user is null) return new Service404Response();

        return new Service200Response(user.Adapt<ProfileResponse>());
    }

    public async Task<ServiceResponse> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == userId.Value);
        if (user is null) return new Service404Response();

        user.Name = request.Name;
        user.Phone = request.Phone;
        user.ModifiedBy = "self";
        user.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the profile."));
        }

        return new Service200Response(user.Adapt<ProfileResponse>());
    }

    public async Task<ServiceResponse> DeactivateAccountAsync(DeactivateAccountRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == userId.Value);
        if (user is null) return new Service404Response();

        var (hash, salt, _) = encryptionService.Extract(user.PasswordHash);
        if (!encryptionService.VerifyPassword(request.CurrentPassword, hash, salt))
        {
            return new Service401Response("Current password is incorrect.");
        }

        user.IsActive = false;
        user.ModifiedBy = "self";
        user.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to deactivate the account."));
        }

        return new Service200Response("Account deactivated successfully.");
    }

    public async Task<ServiceResponse> GetUsersAsync(GetUsersRequest request)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u =>
                u.Name.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase) ||
                u.Email.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        var projected = query.ProjectToType<GetUsersResponse>();

        projected = request.IsOrderDescending
            ? projected.OrderByDescending(s => s.UserId)
            : projected.OrderBy(s => s.UserId);
        var totalRecords = await projected.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await projected.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetUserAsync(GetUserRequest request)
    {
        var result = await DbSet.Where(u => u.UserId == request.UserId).ProjectToType<GetUserResponse>()
            .FirstOrDefaultAsync();

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> ActivateUserAsync(ActivateUserRequest request)
    {
        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == request.UserId);
        if (user is null) return new Service404Response();

        user.IsActive = true;
        user.ModifiedBy = "admin";
        user.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to activate the user."));
        }

        return new Service200Response("User activated successfully.");
    }

    public async Task<ServiceResponse> DeactivateUserAsync(DeactivateUserRequest request)
    {
        var currentUserId = httpContextAccessor.GetCurrentUserId();
        if (currentUserId == request.UserId)
        {
            return new Service400Response("You cannot deactivate your own account through this endpoint.");
        }

        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == request.UserId);
        if (user is null) return new Service404Response();

        user.IsActive = false;
        user.ModifiedBy = "admin";
        user.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to deactivate the user."));
        }

        return new Service200Response("User deactivated successfully.");
    }
}
