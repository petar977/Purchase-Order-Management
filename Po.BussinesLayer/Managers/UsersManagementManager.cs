using DataLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Po.Common.Interfaces;
using Po.Common.Models;
using System.Security.Claims;

namespace BussinesLayer.Managers
{
    public class UsersManagementManager : IUsersManagementManager
    {
        private readonly PoDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersManagementManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, PoDbContext dbContext)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<List<UserRolesViewModel>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (IdentityUser user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.Roles = await GetUserRoles(user);
                thisViewModel.Companies = await GetUserCompanies(user);
                userRolesViewModel.Add(thisViewModel);
            }
            return userRolesViewModel;
        }
        private async Task<List<string>> GetUserRoles(IdentityUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
        private async Task<List<string>> GetUserCompanies(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var companyClaims = userClaims.Where(x => x.Type == "Company");
            List<string> Companies = [];
            foreach (var i in _dbContext.Companies)
            {
                foreach(var claims in companyClaims)
                {
                    if(i.Id.ToString() == claims.Value)
                    {
                        Companies.Add(i.Name);
                    }
                }
            }           
            return Companies;
        }
        public async Task<List<ManageUserRolesViewModel>> GetRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return model;
        }
        public async Task<JsonResponse> ChangeRole(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new JsonResponse() { Message = "User not found!" };
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                return new JsonResponse() { Message = "Cannot remove user existing roles" };
            }
            var newRoles = model.Where(x => x.Selected).Select(y => y.RoleName);
            result = await _userManager.AddToRolesAsync(user, newRoles);
            await _userManager.UpdateSecurityStampAsync(user);

            if (!result.Succeeded)
            {
                return new JsonResponse() { Message = "Cannot add selected roles to user" };
            }
            return new JsonResponse() { Message=$"Role changed to {user.UserName}",Success=true};
        }
        public async Task<List<ManageUserCompaniesViewModel>> GetCompanies (string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var model = new List<ManageUserCompaniesViewModel>();
            var claims = await _userManager.GetClaimsAsync(user);
            var companyClaims = claims.Where(x => x.Type == "Company").ToList();
            foreach (var company in _dbContext.Companies)
            {
                var userCompaniesViewModel = new ManageUserCompaniesViewModel
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    Status = company.Status
                };              
                foreach (var claim in companyClaims)
                {
                    if (claim.Value == company.Id.ToString())
                    {
                        userCompaniesViewModel.Selected = true;
                        break;
                    }
                    else
                    {
                        userCompaniesViewModel.Selected = false;
                    }
                }
                model.Add(userCompaniesViewModel);
            }
            return model;
        }
        public async Task<JsonResponse> ManageCompanies(List<ManageUserCompaniesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new JsonResponse() { Message="User not found!"};
            }
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            var companies = model.Where(x => x.Selected);
            if (!result.Succeeded)
            {
                return new JsonResponse() { Message= "Cannot remove user existing roles" };
            }
            List<Claim> AllClaims = new List<Claim>();
            foreach (var i in companies)
            {
                var some = new Claim("Company", i.CompanyId.ToString());
                AllClaims.Add(some);
            }
            result = await _userManager.AddClaimsAsync(user, AllClaims);
            if (!result.Succeeded)
            {
                return new JsonResponse() { Message = "Cannot add selected roles to user" };
            }
            return new JsonResponse() { Message=$"{user.UserName} companies successfully changed!", Success=true};
        }
        public async Task<AddUserViewModel> GetUsers()
        {
            var model = new AddUserViewModel();
            var roles = new List<ManageUserRolesViewModel>();
            var companies = new List<ManageUserCompaniesViewModel>();
            await foreach (var role in _dbContext.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Selected = false
                };
                
                roles.Add(userRolesViewModel);
            }
            await foreach (var company in _dbContext.Companies)
            {
                var userCompaniesViewModel = new ManageUserCompaniesViewModel
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    Status = company.Status,
                    Selected = false
                };
                companies.Add(userCompaniesViewModel);
            }
            model.Roles = roles;
            model.Companies = companies;
            return model;
        }
        public async Task<IdentityResult> CreateUser(AddUserViewModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) {
                var companies = _dbContext.Companies.Where(x => model.Company.Contains(x.Id));
                List<Claim> AllClaims = new List<Claim>();
                foreach (var i in companies)
                {
                    var claim = new Claim("Company", i.Id.ToString());
                    AllClaims.Add(claim);
                }
                result = await _userManager.AddClaimsAsync(user, AllClaims);
                var roles = model.Roles.Where(x => x.Selected).Select(y => y.RoleName);
                result = await _userManager.AddToRolesAsync(user, roles);
                return result;
            }            
            return result;
        }
        public async Task<JsonResponse> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new JsonResponse() { Message="User not found.", Success=false};
            }
            await _userManager.DeleteAsync(user);
            return new JsonResponse() { Message="User has been deleted.", Success=true};           
        }
    }
}