using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;

namespace WebApplication1.Services.AddressService
{
    public interface IAddressService
    {
        Task<ApiResponseDto<AddressResponseDto>> CreateAddressAsync(CreateAddressRequestDto createAddressRequestDto, int userId);

        Task<ApiResponseDto<AddressResponseDto>> UpdateAddressAsync(UpdateAddressRequestDto updateAddressRequestDto, int addressId, int userId);

        Task<ApiResponseDto<string>> DeleteAddressAsync(int addressId, int userId);

        Task<ApiResponseDto<AddressResponseDto>> GetAddressByIdAsync(int addressId, int userId);

        Task<ApiResponseDto<IEnumerable<AddressResponseDto>>> GetUserAddressesAsync(int userId);
    }
}
