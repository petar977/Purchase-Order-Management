using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Po.Common.Interfaces;
using Po.Common.Models.Dto;
using Po.Common.Utilities;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize(Roles =Roles.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyManager _companyManager;
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(ICompanyManager companyManager, ILogger<CompanyController> logger)
        {
            _companyManager = companyManager;
            _logger = logger;
        }
        public IActionResult Index()
        {           
            var model = _companyManager.GetCompanies();
            return View(model);           
        }
        [HttpPost]
        public IActionResult Add([FromBody]CompanyDto name)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid data";
                return Json(new {message="Invalid data"});
            }
            var obj = _companyManager.AddCompany(name);
            TempData["success"] = obj.Message;
            return Json(obj);
        }
        [HttpGet]
        public IActionResult GetCompany(int id) 
        {
            var obj = _companyManager.GetCompany(id);
            if (obj == null)
            {
                return NotFound();
            }
            return Json(obj);
        }

        [HttpPost]
        public IActionResult EditCompany(CompanyDto modelDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { message = "Invalid data" });
            }
            var obj = _companyManager.EditCompany(modelDto);
            if (obj.Success == false)
            {
                TempData["error"] = obj.Message;
            }
            TempData["success"] = obj.Message;
            return Json(obj);
        }
        [HttpPost]
        public IActionResult ChangeStatus(int id)
        {
            var obj = _companyManager.ChangeStatus(id);
            if(obj.Success == false)
            {
                TempData["error"] = obj.Message;
            }
            TempData["success"] = obj.Message;
            return Json(obj);
        }
    }
}
