using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Models.Models;
using Store.Models.Models.ViewModels;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAllWithCategory().ToList();

            return View(products);
        }

        // GET: ProductController/Create
        public ActionResult Upsert(int? Id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll()
                                                    .Select(u => new SelectListItem
                                                    {
                                                        Text = u.Name,
                                                        Value = u.Id.ToString()
                                                    }),
                Product = new Product()
            };
            if(Id == null || Id == 0)
            {
                // Create Product
                return View(productVM);
            }
            else
            {
                // Update Product
                productVM.Product = _unitOfWork.ProductRepository.Get(p=>p.Id == Id);
                return View(productVM);
            }
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";

                }
                else
                {
                    // Model State Fix Issues
                    //productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
                    //{
                    //    Text = c.Name,
                    //    Value = c.Id.ToString()
                    //});
                    //return View(productVM);
                    TempData["error"] = GetModelErrors();

                }
            }
            catch
            {
            }
            return RedirectToAction("Index");

        }


        // GET: ProductController/Delete/5
        public ActionResult Delete(int? id)
        {
            Product? productToDelete = _unitOfWork.ProductRepository.Get(c=>c.Id==id);
            if (productToDelete == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction("Index");

            }
            return View(productToDelete);
        }

        // POST: ProductController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePOST(int? id)
        {
            var productToDelete = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (productToDelete == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction("Index");

            }
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.ProductRepository.Remove(productToDelete);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Product deleted successfully"; 
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = GetModelErrors();
                return View();
            }
        }

        private string GetModelErrors()
        {
            var errors = new StringBuilder();
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.AppendLine($"- Error:{error.ErrorMessage}\n");
                }
            }
            return errors.ToString();
        }
    }
}
