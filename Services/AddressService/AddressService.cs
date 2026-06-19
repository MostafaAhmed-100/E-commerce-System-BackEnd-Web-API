using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.AddressRepository;

namespace WebApplication1.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _addressRepository = addressRepository;
        }

        public async Task<ApiResponseDto<AddressResponseDto>> CreateAddressAsync(CreateAddressRequestDto createAddressRequestDto, int userId)
        {
            var address = _mapper.Map<Address>(createAddressRequestDto);
            address.UserId = userId;
            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<AddressResponseDto>
            {
                Message = "Address created successfully.",
                Data = _mapper.Map<AddressResponseDto>(address)
            };
        }

        public async Task<ApiResponseDto<AddressResponseDto>> UpdateAddressAsync(UpdateAddressRequestDto updateAddressRequestDto, int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                throw new NotFoundException("The address does not exist.");
            }

            if (address.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to update this address.");
            }

            _mapper.Map<Address>(updateAddressRequestDto);


            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<AddressResponseDto>
            {
                Message = "Address updated successfully.",
                Data = _mapper.Map<AddressResponseDto>(address)
            };
        }

        public async Task<ApiResponseDto<string>> DeleteAddressAsync(int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                throw new NotFoundException("The address does not exist.");
            }

            if (address.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to delete this address.");
            }

            _addressRepository.Delete(address);
            await _addressRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Message = "Address deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<AddressResponseDto>> GetAddressByIdAsync(int addressId, int userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
            {
                throw new NotFoundException("The address does not exist.");
            }

            if (address.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to view this address.");
            }

            return new ApiResponseDto<AddressResponseDto>
            {
                Message = "Address retrieved successfully.",
                Data = _mapper.Map<AddressResponseDto>(address)
            };
        }

        public async Task<ApiResponseDto<IEnumerable<AddressResponseDto>>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);

            return new ApiResponseDto<IEnumerable<AddressResponseDto>>
            {
                Message = "User addresses retrieved successfully.",
                Data = _mapper.Map<IEnumerable<AddressResponseDto>>(addresses)
            };
        }
    }
}