namespace WebApplication1.Repository.Interface
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetById(int id);
        public Task<List<Category>> GetAll();
        public Task<Category> Create(Category category);
        public Task<Category?> UpdateById(int ID, Category category);
        public Task<bool?> DeleteById(int id);
    }
}
