using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Models.Dto;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ItemsController : Controller
    {
        private readonly IItemsManager _itemsManager;
        public ItemsController(IItemsManager itemsManager)
        {
            _itemsManager = itemsManager;
        }
        public async Task<IActionResult> Index()
        {
            var item = await _itemsManager.Index(User.Identity.Name);
            return View(item);
        }
        [HttpPost]
        public IActionResult GetAllItems(FilterItems filter)
        {
            var results = _itemsManager.GetItems(filter);
            return Json(results);
        }
        [HttpPost]
        public IActionResult Add([FromBody] ItemsDto item)
        {
            if (!ModelState.IsValid)
            {
                return Json(new{success=false,message="Invalid data" });
            }
            _itemsManager.Add(item);
            return Json(new {success=true, message="Item added"});
        }
        public List<ItemsDto> AutoComplete(string q)
        {
            var autocomplete = _itemsManager.AutoComplete(q);
            return autocomplete;
        }
        
        public IActionResult GetItemData(int id)
        {
            var obj = _itemsManager.GetItem(id);
            if (obj == null)
            {
                return Json(new { Message = "Item not found!" });
            }
            return Json(obj);
        }
        [HttpPost]
        public IActionResult PostItemData([FromBody]ItemsDto modelDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Message = "Invalid data" });
            }
            var obj = _itemsManager.EditItem(modelDto);
            return Json(obj);
        }
        [HttpDelete]
        public IActionResult DeleteItem(int id)
        {
            var item = _itemsManager.Delete(id);
            return Json(item);
        }
    }
}
