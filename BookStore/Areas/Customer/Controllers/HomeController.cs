using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Models.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")] 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(IncludeProperties:"Category");
            return View(productList);
        }

        [HttpGet]
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.ProductRepository.Get(u => u.Id == productId, IncludeProperties:"Category"),
                Count = 1,
                ProductId = productId

            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddToCart(int productId)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                
                var product = _unitOfWork.ProductRepository.Get(x => x.Id == productId);

                if (product == null)
                {
                    return NotFound(); // Handle case where product with given ID is not found
                }

                // Check if there's an existing cart for the user
                ShoppingCart cart = _unitOfWork.ShoppingCartRepository.Get(x => x.ApplicationUserId == userId && x.ProductId==productId);

                if (cart != null)
                {
                    // Update existing shopping cart
                    cart.Count += 1;
                    _unitOfWork.ShoppingCartRepository.Update(cart); // Update existing cart
                }
                else
                {
                    // Create new shopping cart
                    cart = new ShoppingCart
                    {
                        ApplicationUserId = userId,
                        ProductId = productId,
                        Count = 1
                    };
                    _unitOfWork.ShoppingCartRepository.Add(cart); // Add new cart
                }

                _unitOfWork.Save();
                TempData["success"] = "Product added successfully";

                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle specific scenarios
                Console.WriteLine("DbUpdateException: " + ex.Message);
                TempData["error"] = "An error occurred while adding the product to the cart.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle other exceptions if necessary
                Console.WriteLine("Exception: " + ex.Message);
                TempData["error"] = "An unexpected error occurred.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCartRepository.Get(x=>x.ApplicationUserId == userId &&
            x.ProductId == shoppingCart.ProductId);
            if (cartFromDb != null)
            {
                // shopping cart exists
                cartFromDb.Count += shoppingCart.Count; // won't update without using Update method because the cart no tracked by default.
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);

            }
            else
            {
                // add cart
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);

            }
            TempData["success"] = "Cart updated successfully";
            _unitOfWork.Save();

            return RedirectToAction("Index"); 
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetProducts(int page = 1, int pageSize = 4)
        {
            var totalProducts = _unitOfWork.ProductRepository.GetAll().Count();
            var products = _unitOfWork.ProductRepository.GetAll()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var cardsHtml = products.Select(p => $@"
             <div class='col-md-3 col-sm-6 border-primary'>
            <div class='card h-100 border-white border-1 rounded-4 shadow-lg'>
                <img src='{p.ImageUrl}' class='card-img-top rounded-4 card-shadow'/>
                <div class='card-body text-center'>
                    <h5 class='card-title'>{p.Title}</h5>
                    <p class='card-text'>{p.Author}</p>
                    <p class='card-text text-warning'>{p.Price100.ToString("C")}</p>
                    <button class='btn btn-outline-warning'><i class='bi bi-cart-plus'></i> Add to cart</button>
                </div>
            </div>
        </div>").Aggregate((current, next) => current + next);

            return Json(new
            {
                cardsHtml,
                totalPages
            });
        }

       

        #endregion
    }
}
