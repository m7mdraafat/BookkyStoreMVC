using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Add(T entity);
    }
}
