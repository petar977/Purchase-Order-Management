using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Utilities;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize(Roles = Roles.Role_Admin)]
    public class UsersManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUsersManagementManager _usersManagementManager;
        public UsersManagementController(UserManager<IdentityUser> userManager, IUsersManagementManager usersManagementManager)
        {
            _usersManagementManager = usersManagementManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {          
            var userRolesViewModel = _usersManagementManager.GetAllUsers();
            return View(await userRolesViewModel);
        }
        public async Task<IActionResult> Manage(string userId)
        {            
            var model = await _usersManagementManager.GetRoles(userId);
            if(model == null)
            {
                TempData["error"] = "User not found";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var obj = await _usersManagementManager.ChangeRole(model, userId);
            if (obj.Success != true)
            {
                TempData["error"] = obj.Message;
            }
            TempData["success"] = obj.Message;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddCompanies(string userId)
        {
            var model = await _usersManagementManager.GetCompanies(userId);
            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddCompanies(List<ManageUserCompaniesViewModel> model, string userId)
        {
            var company = await _usersManagementManager.ManageCompanies(model, userId);
            if (company.Success == true)
            {
                TempData["success"] = company.Message;
                return RedirectToAction("Index");
            }
            TempData["error"] = company.Message;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddUsers()
        {
            var model = await _usersManagementManager.GetUsers();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddUsers(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = await _usersManagementManager.CreateUser(model);
                if (obj.Succeeded)
                {
                    TempData["success"] = "New user created";
                    return RedirectToAction("Index");
                }
                foreach (var error in obj.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(await _usersManagementManager.GetUsers());
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string deleteId)
        {
            var jsonResponse = await _usersManagementManager.DeleteUser(deleteId);
            if (jsonResponse.Success == true)
            {
                TempData["success"] = jsonResponse.Message;
                return Json(jsonResponse);
            }          
            TempData["error"] = jsonResponse.Message;
            return Json(jsonResponse);
        }
    }  
}
