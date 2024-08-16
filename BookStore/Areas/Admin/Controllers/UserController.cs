using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models.Models;
using Store.Models.Models.ViewModels;
using Store.Utility;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var usersList = _db.ApplicationUsers
                                .Include(u => u.Company)
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



