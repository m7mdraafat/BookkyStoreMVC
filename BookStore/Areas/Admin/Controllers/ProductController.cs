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
        private readonly IWebHostEnvironment _webHostEnvironment; // access wwwroot
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
            //return View(products);
        }

        // GET: ProductController/Create
        [HttpGet]
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

                    // save image
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Get file name with extension
                        string productPath = Path.Combine(wwwrootPath, @"Images\Product"); // Path to product folder

                        if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                        {
                            // Delete the old image 
                            var oldImagePath = Path.Combine(wwwrootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        if (!Directory.Exists(productPath))
                        {
                            Directory.CreateDirectory(productPath); // Create directory if it doesn't exist
                        }

                        string filePath = Path.Combine(productPath, fileName); // Full path to save the file

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream); // Save the file to disk
                        }

                        // Update productVM with the relative path to the saved image
                        productVM.Product.ImageUrl = @"\Images\Product\" + fileName;
                    }

                    if(productVM.Product.Id == 0)
                    {
                        _unitOfWork.ProductRepository.Add(productVM.Product);
                        TempData["sucess"] = "Product created successfully";
                    }
                    else
                    {
                        _unitOfWork.ProductRepository.Update(productVM.Product);
                        TempData["sucess"] = "Product updated successfully";
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                      
                }
                else
                {
                    // Model State Fix Issues

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

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 3)
        {
            try
            {
                var totalProducts = _unitOfWork.ProductRepository.GetAll().Count();
                var products = _unitOfWork.ProductRepository.GetAll(IncludeProperties: "Category")
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Json(new
                {
                    data = products,
                    totalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
