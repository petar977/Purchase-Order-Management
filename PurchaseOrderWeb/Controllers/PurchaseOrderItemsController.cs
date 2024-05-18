using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Po.Common.Interfaces;
using Po.Common.Models.Dto;
using Po.Common.Utilities;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize]
    public class PurchaseOrderItemsController : Controller
    {
        private readonly IPurchaseOrderItemsManager _purchaseOrderItemsManager;
        public PurchaseOrderItemsController(IPurchaseOrderItemsManager purchaseOrderItemsManager)
        {
            _purchaseOrderItemsManager = purchaseOrderItemsManager;
        }
        [HttpGet]
        public IActionResult GetItems(int id)
         {
            var list = _purchaseOrderItemsManager.GetItems(id);;
            return Json(new { data = list});
        }
        [HttpPost]
        public IActionResult Add([FromBody]PurchaseOrderItemsDto item)
        {
            if(!ModelState.IsValid)
            {
                return Json(new { Message = "Invalid data", Success = false });
            }
            var obj = _purchaseOrderItemsManager.AddItem(item,User.Identity.Name,User.IsInRole(Roles.Role_Admin));
            return Json(obj);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var obj = _purchaseOrderItemsManager.Delete(id, User.Identity.Name, User.IsInRole(Roles.Role_Admin));
            return Json(obj);
        }
        [HttpPost]
        public IActionResult Clone(int cloneId, int currId)
        {
            var obj = _purchaseOrderItemsManager.Clone(cloneId, currId, User.Identity.Name, User.IsInRole(Roles.Role_Admin));
            return Json(obj);
        }
        [HttpDelete]
        public IActionResult DeleteAll(int orderId)
        {
            var obj = _purchaseOrderItemsManager.DeleteAll(orderId,User.Identity.Name,User.IsInRole(Roles.Role_Admin));
            return Json(obj);
        }
        public IActionResult GetItem(int id)
        {
            var model = _purchaseOrderItemsManager.GetItem(id);
            return Json(model);
        }
        [HttpPost]
        public IActionResult EditItem(string name, int qty, double price, int itemId, string link)
        {           
            var obj = _purchaseOrderItemsManager.EditItem(name, qty, price, itemId,link);
            return Json(obj);
        }
        public async Task<IActionResult> GetCloneData(string userName)
        {
            var comp = await _purchaseOrderItemsManager.GetCloneComp(userName);

            return Json(new {data = comp.Companies});
        }
    }
}
