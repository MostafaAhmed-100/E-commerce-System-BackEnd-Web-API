using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Repository.SpecificRepository.AddressRepository;

namespace WebApplication1.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<ApiResponseDto<AddressResponseDto>> CreateAddressAsync(CreateAddressRequestDto createAddressRequestDto, int userId)
        {
            var address = new Address
            {
                UserId = userId,
                City = createAddressRequestDto.City,
                State = createAddressRequestDto.State,
                HomeAddress = createAddressRequestDto.HomeAddress,
                Zip_Code = createAddressRequestDto.ZipCode
            };

            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<AddressResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Address created successfully.",
                Data = new AddressResponseDto
                {
                    AddressId = address.AddressId,
                    City = address.City,
                    State = address.State,
                    HomeAddress = address.HomeAddress,
                    ZipCode = address.Zip_Code
                }
            };
        }

        public async Task<ApiResponseDto<AddressResponseDto>> UpdateAddressAsync(UpdateAddressRequestDto updateAddressRequestDto, int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                return new ApiResponseDto<AddressResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "ADDRESS_NOT_FOUND",
                    Data = null,
                    Message = "The address does not exist."
                };
            }

            if (address.UserId != userId)
            {
                return new ApiResponseDto<AddressResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to update this address."
                };
            }

            address.City = updateAddressRequestDto.City;
            address.State = updateAddressRequestDto.State;
            address.HomeAddress = updateAddressRequestDto.HomeAddress;
            address.Zip_Code = updateAddressRequestDto.ZipCode;

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<AddressResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Address updated successfully.",
                Data = new AddressResponseDto
                {
                    AddressId = address.AddressId,
                    City = address.City,
                    State = address.State,
                    HomeAddress = address.HomeAddress,
                    ZipCode = address.Zip_Code
                }
            };
        }

        public async Task<ApiResponseDto<string>> DeleteAddressAsync(int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "ADDRESS_NOT_FOUND",
                    Data = null,
                    Message = "The address does not exist."
                };
            }

            if (address.UserId != userId)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to delete this address."
                };
            }

            _addressRepository.Delete(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Address deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<AddressResponseDto>> GetAddressByIdAsync(int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                return new ApiResponseDto<AddressResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "ADDRESS_NOT_FOUND",
                    Data = null,
                    Message = "The address does not exist."
                };
            }

            if (address.UserId != userId)
            {
                return new ApiResponseDto<AddressResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to view this address."
                };
            }

            return new ApiResponseDto<AddressResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Address retrieved successfully.",
                Data = new AddressResponseDto
                {
                    AddressId = address.AddressId,
                    City = address.City,
                    State = address.State,
                    HomeAddress = address.HomeAddress,
                    ZipCode = address.Zip_Code
                }
            };
        }

        public async Task<ApiResponseDto<IEnumerable<AddressResponseDto>>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);

            return new ApiResponseDto<IEnumerable<AddressResponseDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "User addresses retrieved successfully.",
                Data = addresses.Select(a => new AddressResponseDto
                {
                    AddressId = a.AddressId,
                    City = a.City,
                    State = a.State,
                    HomeAddress = a.HomeAddress,
                    ZipCode = a.Zip_Code
                }).ToList()
            };
        }
    }
}