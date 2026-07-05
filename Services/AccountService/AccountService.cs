using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;

namespace WebApplication1.Services.AccountService
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<User> _userManager;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly ILogger<AccountService> _logger;
        public AccountService
        (
            ILogger<AccountService> logger,
            UserManager<User> userManager,
            IBuyerRepository buyerRepository,
            ISellerRepository sellerRepository
        )
        {
            _logger = logger;
            _userManager = userManager;
            _buyerRepository = buyerRepository;
            _sellerRepository = sellerRepository;
        }
        public async Task<ApiResponseDto<string>> DeleteAccountAsync(int userId)
        {
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
            {
                _logger.LogWarning("User {UserId} account Not Found ", userId);
                throw new NotFoundException("User not found");
            }
            await _userManager.SetLockoutEnabledAsync(User, enabled: true);
            await _userManager.SetLockoutEndDateAsync(User, DateTimeOffset.MaxValue);
            bool isBuyer = await _userManager.IsInRoleAsync(User, "Buyer");
            bool IsSeller = await _userManager.IsInRoleAsync(User, "Seller");
            if (isBuyer)
            {
                var Buyer = await _buyerRepository.GetBuyerByUserId(userId);
                Buyer.IsDeleted = true;
                await _buyerRepository.SaveChangesAsync();
            }
            if (IsSeller)
            {
                var Seller = await _sellerRepository.GetSellerIdByUserId(userId);
                Seller.IsDeleted = true;
                await _sellerRepository.SaveChangesAsync();
            }
            _logger.LogInformation("User {UserId} account locked and profile soft-deleted. IsBuyer: {IsBuyer}, IsSeller: {IsSeller}", userId, isBuyer, IsSeller);
            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "User Deleted Succesfuly"
            };
        }
    }
}
