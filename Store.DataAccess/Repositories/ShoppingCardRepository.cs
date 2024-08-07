using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;
using Microsoft.EntityFrameworkCore; 
namespace Store.DataAccess.Repositories
{
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository
    {
        private readonly AppDbContext _db;
        public ShoppingCardRepository(AppDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public void Update(ShoppingCard shoppingCard)
        {
            _db.Entry(shoppingCard).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
