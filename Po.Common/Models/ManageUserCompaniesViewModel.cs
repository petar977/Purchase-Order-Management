using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class ManageUserCompaniesViewModel
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool Selected { get; set; }
        public Status Status { get; set; }
    }
}
