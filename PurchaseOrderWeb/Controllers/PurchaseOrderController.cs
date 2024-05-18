using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Po.Common.Interfaces;
using Po.Common.Models.Dto;
using Po.Common.Utilities;
using Po.Common.Models;
using DataLayer.Data;
using Microsoft.AspNetCore.Identity;

namespace PurchaseOrdersManagement.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderManager _purchaseOrderManager;
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly PoDbContext _db;
        private readonly SignInManager<IdentityUser> _signIn;
        public PurchaseOrderController(IPurchaseOrderManager purchaseOrderManager, ILogger<PurchaseOrderController> logger,PoDbContext db,SignInManager<IdentityUser> signIn)
        {
            _purchaseOrderManager = purchaseOrderManager;
            _logger = logger;
            _db = db;
            _signIn = signIn;
        }
        public async Task<IActionResult> Index()
        {
          
            var sas = User.Claims.FirstOrDefault(x=>x.Type == "AspNet.Identity.SecurityStamp").Value;
            var user = _db.Users.FirstOrDefault(x=>x.Email == User.Identity.Name);
            if(sas != user.SecurityStamp)
            {
                await _signIn.SignOutAsync();
                return RedirectToAction("Index");
            }
            var model = await _purchaseOrderManager.GetComp(User.Identity.Name);
            return View(model); 
        }
        [HttpPost]
        public IActionResult GetAll(PurchaseOrderFilter filter)
        {           
            var obj = _purchaseOrderManager.GetAll(filter,User.Identity.Name);
            return Json(obj.Result);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = await _purchaseOrderManager.GetComp(User.Identity.Name);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddPages()
        {
            var model = await _purchaseOrderManager.GetComp(User.Identity.Name);
            return Json(new { data = model });
        }

        [HttpPost]
        public IActionResult Add(PurchaseOrderDto order)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Message = "Invalid data", Success = false });
            }
            var baseurl = HttpContext.Request.Host;
            var obj = _purchaseOrderManager.Add(order,User.Identity.Name,baseurl);
            
            return Json(obj);           
        }
        public IActionResult Edit(int id)
        {
            var obj = _purchaseOrderManager.Get(id,User.Identity.Name,User.IsInRole(Roles.Role_Admin));
            if(obj == null)
            {
                TempData["error"] = "Order not found";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(PurchaseOrderDto order)
        {   
            if(!ModelState.IsValid)
            {
                return Json(new { message = "Invalid data", success = false });
            }
            var obj = _purchaseOrderManager.Update(order,User.Identity.Name,User.IsInRole(Roles.Role_Admin));

            return Json(obj);
        }
        [Authorize(Roles =Roles.Role_Admin)]
        public async Task<IActionResult> Approve(int id)
        {
            var baseurl = HttpContext.Request.Host;
            var form = HttpContext.Request;
            var obj = await _purchaseOrderManager.ChangeStatus(id, State.Approved,User.Identity.Name, User.IsInRole(Roles.Role_Admin), baseurl);
            TempData["success"] = obj.Message;
            return Json(obj);
        }
        [Authorize(Roles = Roles.Role_Admin)]
        public IActionResult Deny(int id)
        {
            var baseurl = HttpContext.Request.Host;
            var form = HttpContext.Request;
            var obj = _purchaseOrderManager.ChangeStatus(id, State.Denied, User.Identity.Name, User.IsInRole(Roles.Role_Admin), baseurl);
            TempData["success"] = obj.Result.Message;
            return Json(obj.Result);
        }
        public async Task<IActionResult> Cancel(int id)
        {
            var baseurl = HttpContext.Request.Host;
            var form = HttpContext.Request;
            var obj = await _purchaseOrderManager.ChangeStatus(id, State.Canceled, User.Identity.Name, User.IsInRole(Roles.Role_Admin), baseurl);
            if (obj.Success == true)
            {
                TempData["success"] = obj.Message;
            }
            return Json(obj);
        }
        public async Task<IActionResult> Pending(int id)
        {
            var baseurl = HttpContext.Request.Host;
            var form = HttpContext.Request;
            var obj = await _purchaseOrderManager.ChangeStatus(id, State.Pending, User.Identity.Name, User.IsInRole(Roles.Role_Admin), baseurl);
            if(obj.Success == true)
            {
                TempData["success"] = obj.Message;
            }           
            return Json(obj);
        }
        public IActionResult InProgress(int id)
        {
            var baseurl = HttpContext.Request.Host;
            var form = HttpContext.Request;
            var obj = _purchaseOrderManager.ChangeStatus(id, State.InProgress, User.Identity.Name, User.IsInRole(Roles.Role_Admin), baseurl);
            if (obj.Result.Success == true)
            {
                TempData["success"] = obj.Result.Message;
            }
            return Json(obj.Result);
        }
        public IActionResult TestPartial()
        {
            return PartialView("_AddItemsPartial");
        }
        public IActionResult GetInfo(int id)
        {
            var info = _purchaseOrderManager.GetInfo(id);
            return Json(new {data = info});
        }
    }
}
