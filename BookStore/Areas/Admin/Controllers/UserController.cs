using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;
using Store.Models.Models.ViewModels;
using Store.Utility;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            AppDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
          
            RoleManagementVM roleVM = new()
            {
                UserName = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId, IncludeProperties:"Company").Name,
                CurrentRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId))
                                                                                            .GetAwaiter()
                                                                                            .GetResult().FirstOrDefault(),
                RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }),
                CompanyList = _unitOfWork.CompanyRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                CompanyId = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId).CompanyId
            };
            roleVM.UserId = userId;

            return View(roleVM); 
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleVM)
        {

            var oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUserRepository.Get(u => u.Id == roleVM.UserId))
                                                                                            .GetAwaiter()
                                                                                            .GetResult().FirstOrDefault();
            ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == roleVM.UserId);

            if (!(roleVM.CurrentRole == oldRole))
            {
                // a role was updated
                if(roleVM.CurrentRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleVM.CompanyId;

                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null; 
                }
                applicationUser.Discriminator = roleVM.CurrentRole;

                _unitOfWork.ApplicationUserRepository.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleVM.CurrentRole).GetAwaiter().GetResult();

            }
            else // role => company, and company changed
            {
                if(oldRole == SD.Role_Company && applicationUser.CompanyId != roleVM.CompanyId)
                {
                    applicationUser.CompanyId = roleVM?.CompanyId;
                    _unitOfWork.ApplicationUserRepository.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var usersList = _db.ApplicationUsers
                               .Include(u=>u.Company)
                                .Select(user => new
                                {
                                    Id = user.Id,
                                    Name = user.Name,
                                    Email = user.Email,
                                    PhoneNumber = user.PhoneNumber??"",
                                    Company = user.Company.Name??"",
                                    Role = _db.UserRoles.Where(ur => ur.UserId == user.Id)
                                                        .Select(ur => _db.Roles.FirstOrDefault(r => r.Id == ur.RoleId).Name)
                                                        .FirstOrDefault(),
                                    LockoutEnd = user.LockoutEnd
                                })
                                .ToList();

            return Json(new { success=true, data = usersList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // User is currently locked; unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // User is unlocked; lock them
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "User Locked/Unlocked Successfully" });
        }


        #endregion

    }
}



