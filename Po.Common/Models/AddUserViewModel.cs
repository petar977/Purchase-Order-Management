using DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace Po.Common.Models
{
    
    public class AddUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public List<ManageUserRolesViewModel> Roles { get; set; }
        public List<ManageUserCompaniesViewModel>? Companies { get; set; }
        public List<int> Company { get; set; }
    }
}
