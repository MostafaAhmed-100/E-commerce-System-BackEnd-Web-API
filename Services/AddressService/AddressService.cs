using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.AddressRepository;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressService> _logger;

        public AddressService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AddressService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<AddressResponseDto>> CreateAddressAsync(CreateAddressRequestDto createAddressRequestDto, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var address = _mapper.Map<Address>(createAddressRequestDto);
                address.UserId = userId;
                await _unitOfWork.AddressRepository.AddAsync(address);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User {UserId} successfully created a new address {AddressId}.", userId, address.AddressId);
                await transaction.CommitAsync();
                return new ApiResponseDto<AddressResponseDto>
                {
                  
                    Message = "Address created successfully.",
                    Data = _mapper.Map<AddressResponseDto>(address)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while Creating Address for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<AddressResponseDto>> UpdateAddressAsync(UpdateAddressRequestDto updateAddressRequestDto, int addressId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
                if (address == null)
                {
                    _logger.LogWarning("User {UserId} attempted to update non-existent Address {AddressId}.", userId, addressId);
                    throw new NotFoundException("The address does not exist.");
                }

                if (address.UserId != userId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to update Address {AddressId} belonging to another user.", userId, addressId);
                    throw new UnauthorizedException("You do not have permission to update this address.");
                }

                _mapper.Map(updateAddressRequestDto, address);

                _unitOfWork.AddressRepository.Update(address);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User {UserId} successfully updated Address {AddressId}.", userId, addressId);
                await transaction.CommitAsync();
                return new ApiResponseDto<AddressResponseDto>
                {
                    Message = "Address updated successfully.",
                    Data = _mapper.Map<AddressResponseDto>(address)
                };
            }
            catch(Exception ex) 
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while Updateing Address for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteAddressAsync(int addressId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);

                if (address == null)
                {
                    _logger.LogWarning("User {UserId} attempted to delete non-existent Address {AddressId}.", userId, addressId);
                    throw new NotFoundException("The address does not exist.");
                }

                if (address.UserId != userId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to delete Address {AddressId} belonging to another user.", userId, addressId);
                    throw new UnauthorizedException("You do not have permission to delete this address.");
                }

                _unitOfWork.AddressRepository.Delete(address);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User {UserId} successfully deleted Address {AddressId}.", userId, addressId);
                await transaction.CommitAsync();
                return new ApiResponseDto<string>
                {
                    Message = "Address deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while Deleteing Address for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<AddressResponseDto>> GetAddressByIdAsync(int addressId, int userId)
        {
            try
            {
                var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);

                if (address == null)
                {
                    _logger.LogWarning("User {UserId} attempted to retrieve non-existent Address {AddressId}.", userId, addressId);
                    throw new NotFoundException("The address does not exist.");
                }

                if (address.UserId != userId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to retrieve Address {AddressId} belonging to another user.", userId, addressId);
                    throw new UnauthorizedException("You do not have permission to view this address.");
                }

                return new ApiResponseDto<AddressResponseDto>
                {
                    Message = "Address retrieved successfully.",
                    Data = _mapper.Map<AddressResponseDto>(address)
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Geting Address for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AddressResponseDto>>> GetUserAddressesAsync(int userId)
        {
            try
            {
                var addresses = await _unitOfWork.AddressRepository.FindAsync(x => x.UserId == userId);

                return new ApiResponseDto<IEnumerable<AddressResponseDto>>
                {
                    Message = "User addresses retrieved successfully.",
                    Data = _mapper.Map<IEnumerable<AddressResponseDto>>(addresses)
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while GetUserAddresses for User {UserId}", userId);
                throw;
            }
        }
    }
}