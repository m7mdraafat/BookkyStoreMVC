using NuGet.Protocol.Core.Types;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;
using Microsoft.EntityFrameworkCore; 

namespace Store.DataAccess.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _appDbContext;
        public CompanyRepository(AppDbContext dbContext) : base(dbContext)
        {
            _appDbContext = dbContext;
        }

        public void Update(Company company)
        {
            _appDbContext.Entry(company).State = EntityState.Modified;
            _appDbContext.SaveChanges();
        }
    }
}
