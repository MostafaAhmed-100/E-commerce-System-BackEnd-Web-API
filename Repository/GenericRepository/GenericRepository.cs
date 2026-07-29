using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication1.Data;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.Entitys;

namespace WebApplication1.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _AppDbcontext;

        public GenericRepository(AppDbContext appDbcontext)
        {
            _AppDbcontext = appDbcontext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var GetAll = await _AppDbcontext.Set<T>()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            return GetAll;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _AppDbcontext.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.Set<T>().AsNoTracking();
            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _AppDbcontext.Set<T>().FindAsync(id);
            return entity;
        }
        public async Task AddAsync(T entity)
        {
            var AddEntity =  await _AppDbcontext.Set<T>().AddAsync(entity);
        }

        public T Update(T entity)
        { 
            _AppDbcontext.Set<T>().Update(entity);
            return entity;
        }
        public T Delete(T entity)
        {
            _AppDbcontext.Set<T>().Remove(entity);
            return entity;
        }
       
        public async Task<bool> SaveChangesAsync()
        {
            var savedChanges = await _AppDbcontext.SaveChangesAsync();
            if (savedChanges > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> ApplyPaginationAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var TotalCount = await query.CountAsync();
            var pagedResult = await query
                .Skip (pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (pagedResult, TotalCount);
        }
    }
}
