using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Utility;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name != null && category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 6, string search="")
        {
            var query = _unitOfWork.CategoryRepository.GetAll();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c=>c.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase));
            }
            var totalCategories = query.Count();
            var categories = query.Skip((page-1)*pageSize)
                                  .Take(pageSize)
                                  .ToList();

            return Json(new
            {
                data = categories,
                totalPages = (int)Math.Ceiling((double)totalCategories / pageSize),
                totalCategories = totalCategories // Include totalCategories in the response
            });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var category = _unitOfWork.CategoryRepository.Get(x=>x.Id == id);
            if (category == null)
            {
                return Json( new { success= false, message="Error while deleting"});
            }

            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successfully" });
        } 
        #endregion
    }
}
