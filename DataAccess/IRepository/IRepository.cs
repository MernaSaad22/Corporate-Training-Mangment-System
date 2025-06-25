using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
 
        public interface IRepository<T> where T : class
        {
            Task<T> CreateAsync(T entity);

            Task<T> EditAsync(T entity);

            Task<T> DeleteAsync(T entity);

            Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
                Expression<Func<T, object>>[]? includes = null, bool tracked = true);

            T? GetOne(Expression<Func<T, bool>>? expression = null,
                Expression<Func<T, object>>[]? includes = null, bool tracked = true);

            Task<bool> CommitAsync();
        }
    
}
