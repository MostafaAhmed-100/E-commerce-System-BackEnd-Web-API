using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Repository.SpecificRepository.AddressRepository;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface; 
using WebApplication1.Repository.SpecificRepository.CategoryRepository;
using WebApplication1.Repository.SpecificRepository.CouponRepository;
using WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository;
using WebApplication1.Repository.SpecificRepository.OrderRepository;
using WebApplication1.Repository.SpecificRepository.ProductRepository;
using WebApplication1.Repository.SpecificRepository.RefreshTokenRepository;
using WebApplication1.Repository.SpecificRepository.ReviewRepository;
using WebApplication1.Repository.SpecificRepository.SavedCardRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;
using WebApplication1.Repository.SpecificRepository.WishlistItemRepository;
using WebApplication1.Repository.SpecificRepository.WishlistsRepository;
using WebApplication1.Repository.SpecificRepository.ProductVariantRepository;

namespace WebApplication1.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IAddressRepository AddressRepository { get; private set; }
        public IBuyerRepository BuyerRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICouponRepository CouponRepository { get; private set; }
        public ILoyaltyTransactionRepository LoyaltyTransactionRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IRefreshTokenRepository RefreshTokenRepository { get; private set; }
        public IReviewRepository ReviewRepository { get; private set; }
        public ISavedCardRepository SavedCardRepository { get; private set; }
        public ISellerRepository SellerRepository { get; private set; }
        public IWishlistItemRepository WishlistItemRepository { get; private set; }
        public IWishlistsRepository WishlistsRepository { get; private set; }
        public IProductVariantRepository ProductVariantRepository { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            AddressRepository = new AddressRepository(_context);
            BuyerRepository = new BuyerRepository(_context);
            CartRepository = new CartRepository(_context);
            CategoryRepository = new CategoryRepository(_context); 
            CouponRepository = new CouponRepository(_context);
            LoyaltyTransactionRepository = new LoyaltyTransactionRepository(_context);
            OrderRepository = new OrderRepository(_context);
            ProductRepository = new ProductRepository(_context);
            RefreshTokenRepository = new RefreshTokenRepository(_context);
            ReviewRepository = new ReviewRepository(_context);
            SavedCardRepository = new SavedCardRepository(_context);
            SellerRepository = new SellerRepository(_context);
            WishlistItemRepository = new WishlistItemRepository(_context);
            WishlistsRepository = new WishlistsRepository(_context);
            ProductVariantRepository = new ProductVariantRepository(_context);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}