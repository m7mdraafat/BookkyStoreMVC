using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Models.Models;
using Store.Models.Models.ViewModels;
using Store.Utility;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles= SD.Role_Admin)]


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
                productVM.Product = _unitOfWork.ProductRepository.Get(p=>p.Id == Id, IncludeProperties:"ProductImages");
                return View(productVM);
            }
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (productVM.Product.Id == 0)
                    {
                        _unitOfWork.ProductRepository.Add(productVM.Product);
                        TempData["success"] = "Product created successfully";
                    }
                    else
                    {
                        _unitOfWork.ProductRepository.Update(productVM.Product);
                        TempData["success"] = "Product updated successfully";
                    }
                    _unitOfWork.Save();

                    // save image
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    if (files != null)
                    {

                        foreach (IFormFile file in files)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Get file name with extension
                            string productPath = @"images/products/product-" +productVM.Product.Id;
                            string finalPath = Path.Combine(wwwrootPath, productPath); // Path to product folder

                            if (!Directory.Exists(finalPath))
                            {
                                Directory.CreateDirectory(finalPath); 
                            }

                            using(var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + fileName,
                                ProductId = productVM.Product.Id,
                            };

                            if(productVM.Product.ProductImages == null)
                            {
                                productVM.Product.ProductImages = new List<ProductImage>();
                            }

                            productVM.Product.ProductImages.Add(productImage);

                        }
                        _unitOfWork.ProductRepository.Update(productVM.Product);
                        _unitOfWork.Save(); 

                        
                    }
                    TempData["success"] = "Product created/updated successfully"; 
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
        public IActionResult DeleteImage(int imageId)
        {
            var imageToDeleted = _unitOfWork.ProductImageRepository.Get(u=>u.Id == imageId);
            var productId = imageToDeleted.ProductId;
            if (imageToDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImageRepository.Remove(imageToDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Image deleted successfully"; 
            }

            return RedirectToAction(nameof(Upsert), new { id = productId }); 

        }
        #region old delete actions
        //// GET: ProductController/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    Product? productToDelete = _unitOfWork.ProductRepository.Get(c=>c.Id==id);
        //    if (productToDelete == null)
        //    {
        //        TempData["error"] = "Product not found!";
        //        return RedirectToAction("Index");

        //    }
        //    return View(productToDelete);
        //}

        //// POST: ProductController/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeletePOST(int? id)
        //{
        //    var productToDelete = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        //    if (productToDelete == null)
        //    {
        //        TempData["error"] = "Product not found!";
        //        return RedirectToAction("Index");

        //    }
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _unitOfWork.ProductRepository.Remove(productToDelete);
        //            _unitOfWork.Save();
        //        }
        //        TempData["success"] = "Product deleted successfully"; 
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        TempData["error"] = GetModelErrors();
        //        return View();
        //    }
        //}
        #endregion
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
        public IActionResult GetAll(int page = 1, int pageSize = 3, string search = "")
        {
            try
            {
                var query = _unitOfWork.ProductRepository.GetAll(IncludeProperties: "Category");

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Title.StartsWith(search, StringComparison.OrdinalIgnoreCase));
                }

                var totalProducts = query.Count();
                var products = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Json(new
                {
                    data = products,
                    totalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                    totalProducts = totalProducts
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // if product deleted we need delete all directory of images
           
            string productPath = @"images/products/product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath); // Path to product folder

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
