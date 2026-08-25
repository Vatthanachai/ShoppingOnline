using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Model.Entities;
using ShoppingOnline.Model.Requests.ShippingAddresses;

namespace ShoppingOnline.Service.ShippingAddresses;

public interface IShippingAddressService : IBaseService<ShippingAddress>
{
    Task<ServiceResponse> GetShippingAddressesAsync(GetShippingAddressesRequest request);
    Task<ServiceResponse> GetShippingAddressAsync(GetShippingAddressRequest request);
    Task<ServiceResponse> CreateShippingAddressAsync(CreateShippingAddressRequest request);
    Task<ServiceResponse> UpdateShippingAddressAsync(UpdateShippingAddressRequest request);
    Task<ServiceResponse> DeleteShippingAddressAsync(DeleteShippingAddressRequest request);
}
