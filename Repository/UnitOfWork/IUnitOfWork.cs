using Microsoft.EntityFrameworkCore.Storage;
using WebApplication1.Repository.SpecificRepository.AddressRepository;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;
using WebApplication1.Repository.SpecificRepository.CouponRepository;
using WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository;
using WebApplication1.Repository.SpecificRepository.OrderRepository;
using WebApplication1.Repository.SpecificRepository.ProductRepository;
using WebApplication1.Repository.SpecificRepository.ProductVariantRepository;
using WebApplication1.Repository.SpecificRepository.RefreshTokenRepository;
using WebApplication1.Repository.SpecificRepository.ReviewRepository;
using WebApplication1.Repository.SpecificRepository.SavedCardRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;
using WebApplication1.Repository.SpecificRepository.WishlistItemRepository;
using WebApplication1.Repository.SpecificRepository.WishlistsRepository;

namespace WebApplication1.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAddressRepository AddressRepository { get; }
        IBuyerRepository BuyerRepository { get; }
        ICartRepository CartRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICouponRepository CouponRepository { get; }
        ILoyaltyTransactionRepository LoyaltyTransactionRepository { get; }
        IOrderRepository OrderRepository { get; }
        IProductRepository ProductRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ISavedCardRepository SavedCardRepository { get; }
        ISellerRepository SellerRepository { get; }
        IWishlistItemRepository WishlistItemRepository { get; }
        IWishlistsRepository WishlistsRepository { get; }
        IProductVariantRepository ProductVariantRepository { get; }

        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}