using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Models.Models;
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
            List<Product> products = _unitOfWork.ProductRepository.GetAll().ToList();
            return View(products);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.ProductRepository.Add(product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = GetModelErrors();
                return View("Index");
            }
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int? id)
        {
            Product? productToEdit = _unitOfWork.ProductRepository.Get(c=>c.Id == id);
            if(productToEdit == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction("Index");


            }
            return View(productToEdit);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.ProductRepository.Update(product);
                    _unitOfWork.Save();

                }
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = GetModelErrors(); 
                return View();
            }
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
                TempData["success"] = "Product delete successfully"; 
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
                    errors.AppendLine(error.ErrorMessage);
                }
            }
            return errors.ToString();
        }
    }
}
