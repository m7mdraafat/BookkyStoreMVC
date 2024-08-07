using NuGet.Protocol.Core.Types;
using Store.DataAccess.Data;
using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.IRepositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
       void Update(Company company);
    }
}
