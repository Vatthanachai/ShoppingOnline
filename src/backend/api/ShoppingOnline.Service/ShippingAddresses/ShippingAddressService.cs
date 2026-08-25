using Mapster;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.ShippingAddresses;
using ShoppingOnline.Model.Responses.ShippingAddresses;

namespace ShoppingOnline.Service.ShippingAddresses;

public class ShippingAddressService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : BaseService<ShippingAddress, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IShippingAddressService
{
    public async Task<ServiceResponse> GetShippingAddressesAsync(GetShippingAddressesRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var query = DbSet.Where(a => a.UserId == userId.Value).ProjectToType<GetShippingAddressResponse>();

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.ShippingAddressId)
            : query.OrderBy(s => s.ShippingAddressId);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetShippingAddressAsync(GetShippingAddressRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var result = await DbSet.Where(a => a.ShippingAddressId == request.ShippingAddressId && a.UserId == userId.Value)
            .ProjectToType<GetShippingAddressResponse>()
            .FirstOrDefaultAsync();

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreateShippingAddressAsync(CreateShippingAddressRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        // The very first address a user adds becomes their default automatically, so
        // checkout always has one preselected without asking them to set it explicitly.
        var hasExistingAddress = await DbSet.AnyAsync(a => a.UserId == userId.Value);

        var shippingAddress = new ShippingAddress
        {
            UserId = userId.Value,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            IsDefault = !hasExistingAddress,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
        };

        DbSet.Add(shippingAddress);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the shipping address."));
        }

        return new Service200Response(shippingAddress.Adapt<GetShippingAddressResponse>());
    }

    public async Task<ServiceResponse> UpdateShippingAddressAsync(UpdateShippingAddressRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var shippingAddress = await DbSet.FirstOrDefaultAsync(a =>
            a.ShippingAddressId == request.ShippingAddressId && a.UserId == userId.Value);
        if (shippingAddress is null) return new Service404Response();

        shippingAddress.AddressLine1 = request.AddressLine1;
        shippingAddress.AddressLine2 = request.AddressLine2;
        shippingAddress.City = request.City;
        shippingAddress.State = request.State;
        shippingAddress.PostalCode = request.PostalCode;
        shippingAddress.Country = request.Country;
        shippingAddress.ModifiedBy = "system";
        shippingAddress.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the shipping address."));
        }

        return new Service200Response(shippingAddress.Adapt<GetShippingAddressResponse>());
    }

    public async Task<ServiceResponse> DeleteShippingAddressAsync(DeleteShippingAddressRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var shippingAddress = await DbSet.FirstOrDefaultAsync(a =>
            a.ShippingAddressId == request.ShippingAddressId && a.UserId == userId.Value);
        if (shippingAddress is null) return new Service404Response();

        var wasDefault = shippingAddress.IsDefault;
        DbSet.Remove(shippingAddress);

        if (wasDefault)
        {
            // Keep exactly one default standing so checkout always has one preselected -
            // promote whichever address the user added earliest among what's left.
            var nextDefault = await DbSet
                .Where(a => a.UserId == userId.Value && a.ShippingAddressId != shippingAddress.ShippingAddressId)
                .OrderBy(a => a.ShippingAddressId)
                .FirstOrDefaultAsync();

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
                nextDefault.ModifiedBy = "system";
                nextDefault.ModifiedDate = DateTime.UtcNow;
            }
        }

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to delete the shipping address."));
        }

        return new Service200Response("Shipping address deleted successfully.");
    }

    public async Task<ServiceResponse> SetDefaultShippingAddressAsync(SetDefaultShippingAddressRequest request)
    {
        var userId = httpContextAccessor.GetCurrentUserId();
        if (userId is null) return new Service401Response();

        var addresses = await DbSet.Where(a => a.UserId == userId.Value).ToListAsync();
        var target = addresses.FirstOrDefault(a => a.ShippingAddressId == request.ShippingAddressId);
        if (target is null) return new Service404Response();

        foreach (var address in addresses)
        {
            var shouldBeDefault = address.ShippingAddressId == request.ShippingAddressId;
            if (address.IsDefault == shouldBeDefault) continue;

            address.IsDefault = shouldBeDefault;
            address.ModifiedBy = "system";
            address.ModifiedDate = DateTime.UtcNow;
        }

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to set the default shipping address."));
        }

        return new Service200Response(target.Adapt<GetShippingAddressResponse>());
    }
}
