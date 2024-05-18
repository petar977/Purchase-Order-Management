using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Utilities;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize(Roles =Roles.Role_Admin)]
    public class OrderCountController : Controller
    {
        private readonly IOrderCountManager _orderCountManager;
        public OrderCountController(IOrderCountManager orderCountManager)
        {
            _orderCountManager = orderCountManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetAllCounters()
        {
            return Json(_orderCountManager.GetAllCounters());
        }
        public IActionResult Add()
        {
            return View(_orderCountManager.GetCounterVM());
        }
        [HttpPost]
        public IActionResult Add(CounterViewModel model) 
        {
            _orderCountManager.Create(model);
            return RedirectToAction("Index");
        }
        public IActionResult GetEditData(int id)
        {
            var model = _orderCountManager.GetEditData(id);
            if (model == null)
            {
                return Json(new { Message = "Counter not found!" });
            }
            return Json(model);
        }
        public IActionResult EditData(int id, string firstLetter, int counter, string type)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Message = "Invalid data!" });
            }
            var model = _orderCountManager.Edit(id, firstLetter, counter, type);
            return Json(model);
        }
    }
}
