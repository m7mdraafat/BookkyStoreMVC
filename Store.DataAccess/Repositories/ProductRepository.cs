using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext dbContext;

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllWithCategory()
        {
            IEnumerable<Product> query = dbContext.Products
                                        .Include(p => p.Category);
            return query.ToList();
        }

     
        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {

            IQueryable<Product> query = dbContext.Products.Include(p => p.Category);


            return await query.ToListAsync();
        }

        public void Update(Product product)
        {
            dbContext.Entry(product).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}
