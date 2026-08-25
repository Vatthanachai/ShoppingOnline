using System.Linq.Expressions;

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
using ShoppingOnline.Model.Requests.Vendors;
using ShoppingOnline.Model.Responses.Vendors;

namespace ShoppingOnline.Service.Vendors;

public class VendorService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : BaseService<Vendor, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger,
        httpContextAccessor), IVendorService
{
    public async Task<ServiceResponse> GetVendorsAsync(GetVendorsRequest request)
    {
        Expression<Func<GetVendorsResponse, bool>> predicate = s => true;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = predicate.And(s =>
                s.VendorName.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase) ||
                s.ContactPerson.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase) ||
                s.Email.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        var query = DbContext.Set<Vendor>().ProjectToType<GetVendorsResponse>().Where(predicate);

        query = request.IsOrderDescending
            ? query.OrderByDescending(s => s.VendorName)
            : query.OrderBy(s => s.VendorName);
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageLimit);
        var responseData = await query.Skip((request.PageIndex - 1) * request.PageLimit).Take(request.PageLimit)
            .ToListAsync();

        return new Service200PaginationResponse(responseData, request.PageIndex, request.PageLimit, totalRecords,
            totalPages);
    }

    public async Task<ServiceResponse> GetVendorAsync(GetVendorRequest request)
    {
        var result = await DbContext.Set<Vendor>().ProjectToType<GetVendorResponse>()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId);

        if (result == null) return new Service404Response();

        return new Service200Response(result);
    }

    public async Task<ServiceResponse> CreateVendorAsync(CreateVendorRequest request)
    {
        var emailExists = await DbSet.AnyAsync(v => v.Email == request.Email);
        if (emailExists)
        {
            return new Service409Response("A vendor with this email already exists.");
        }

        var vendor = new Vendor
        {
            VendorName = request.VendorName,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
        };

        DbSet.Add(vendor);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the vendor."));
        }

        return new Service200Response(vendor.Adapt<GetVendorResponse>());
    }

    public async Task<ServiceResponse> UpdateVendorAsync(UpdateVendorRequest request)
    {
        var vendor = await DbSet.FirstOrDefaultAsync(v => v.VendorId == request.VendorId);
        if (vendor is null) return new Service404Response();

        var emailExists = await DbSet.AnyAsync(v => v.Email == request.Email && v.VendorId != request.VendorId);
        if (emailExists)
        {
            return new Service409Response("A vendor with this email already exists.");
        }

        vendor.VendorName = request.VendorName;
        vendor.ContactPerson = request.ContactPerson;
        vendor.Email = request.Email;
        vendor.Phone = request.Phone;
        vendor.ModifiedBy = "system";
        vendor.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the vendor."));
        }

        return new Service200Response(vendor.Adapt<GetVendorResponse>());
    }

    public async Task<ServiceResponse> DeactivateVendorAsync(DeactivateVendorRequest request)
    {
        var vendor = await DbSet.FirstOrDefaultAsync(v => v.VendorId == request.VendorId);
        if (vendor is null) return new Service404Response();

        vendor.IsActive = false;
        vendor.ModifiedBy = "system";
        vendor.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to deactivate the vendor."));
        }

        return new Service200Response("Vendor deactivated successfully.");
    }
}
