using Microsoft.AspNetCore.Identity;
using Po.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Interfaces
{
    public interface IUsersManagementManager
    {
        Task<List<UserRolesViewModel>> GetAllUsers();
        Task<List<ManageUserRolesViewModel>> GetRoles(string userId);
        Task<JsonResponse> ChangeRole(List<ManageUserRolesViewModel> model, string userId);
        Task<List<ManageUserCompaniesViewModel>> GetCompanies(string userId);
        Task<JsonResponse> ManageCompanies(List<ManageUserCompaniesViewModel> model, string userId);
        Task<AddUserViewModel> GetUsers();
        Task<IdentityResult> CreateUser(AddUserViewModel model);
        Task<JsonResponse> DeleteUser(string userId);
    }
}
