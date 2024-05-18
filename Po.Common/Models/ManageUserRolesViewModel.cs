using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class ManageUserRolesViewModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
