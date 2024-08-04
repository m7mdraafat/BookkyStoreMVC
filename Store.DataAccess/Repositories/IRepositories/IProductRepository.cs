using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        IEnumerable<Product> GetAllWithCategory();

    }
}
