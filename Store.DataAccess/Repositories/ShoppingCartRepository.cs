using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;
using Microsoft.EntityFrameworkCore; 
namespace Store.DataAccess.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext _db;
        public ShoppingCartRepository(AppDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public void Update(ShoppingCart shoppingCard)
        {
            _db.Entry(shoppingCard).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
