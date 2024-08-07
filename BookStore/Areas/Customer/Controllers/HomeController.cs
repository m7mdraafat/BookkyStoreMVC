using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models;
using Store.Models.Models;
using System.Diagnostics;

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
            Product productDetails = _unitOfWork.ProductRepository.Get(u=>u.Id == productId, IncludeProperties:"Category");
            return View(productDetails);
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
