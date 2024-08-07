using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: CompanyController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CompanyController/Create
        [HttpGet]
        public ActionResult Upsert(int? id)
        {
            if(id == null || id == 0)
            {
                // create company
                return View(new Company());
            }
            else
            {
                // update company
                var company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
                return View(company);
            }

        }

        // POST: CompanyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (company.Id == 0)
                    {
                        _unitOfWork.CompanyRepository.Add(company);
                        TempData["success"] = "Company created successfully";

                    }
                    else
                    {
                        _unitOfWork.CompanyRepository.Update(company);
                        TempData["success"] = "Company created successfully";

                    }
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(company);
            }
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll(int page=1, int pageSize=3, string query="")
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();

            if (!string.IsNullOrEmpty(query))
            {
                companies = companies.Where(c => c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase));
            }

            var totalCompanies = companies.Count();
            var totalPages = (int)Math.Ceiling(totalCompanies / (double)pageSize);

            companies = companies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                data = companies,
                totalCompanies = totalCompanies,
                totalPages = totalPages
            });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting company" });
            }

            _unitOfWork.CompanyRepository.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfully" });
        }

        #endregion
    }
}
