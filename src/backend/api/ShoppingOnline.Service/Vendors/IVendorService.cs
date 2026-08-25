using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.Vendors;

namespace ShoppingOnline.Service.Vendors;

public interface IVendorService : IBaseService<Vendor>
{
    Task<ServiceResponse> GetVendorsAsync(GetVendorsRequest request);
    Task<ServiceResponse> GetVendorAsync(GetVendorRequest request);
    Task<ServiceResponse> CreateVendorAsync(CreateVendorRequest request);
    Task<ServiceResponse> UpdateVendorAsync(UpdateVendorRequest request);
    Task<ServiceResponse> DeactivateVendorAsync(DeactivateVendorRequest request);
}
