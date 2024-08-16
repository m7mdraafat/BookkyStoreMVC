using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Models.ViewModels
{
    public class RoleManagementVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentRole { get; set; }
        public int? CompanyId { get; set; }
        public IEnumerable<SelectListItem> RoleList{ get; set; } 
        public IEnumerable<SelectListItem>? CompanyList { get; set; }

    }

}
