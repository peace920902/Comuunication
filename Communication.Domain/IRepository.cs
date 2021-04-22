using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Communication.Domain
{
    public interface IRepository<in TId, T> where T: Entity<TId>
    {
        IQueryable<T> GetAll();
        Task<T> FindAsync(TId id);
        Task<T> CreateAsync(T item);
        Task<T> UpdateAsync(TId id, T item);
        Task<bool> DeleteAsync(TId id);
    }
}