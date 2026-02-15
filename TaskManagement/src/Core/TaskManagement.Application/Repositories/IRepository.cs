using System.Linq.Expressions;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<PaginatedResponseDto<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PaginatedResponseDto<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate);
    }
}
