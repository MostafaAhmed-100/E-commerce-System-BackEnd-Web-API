namespace WebApplication1.Repository.Interface
{
    public interface ICartRepository
    {
        public Task<List<Cart>> GetAll();

        public Task<Cart?> GetById(int id);

        public Task<Cart> Create(Cart cart);

        public Task<Cart?> UpdateById(int Id ,Cart cart);

        public Task<bool?> DeleteById(int Id);
    }
}
