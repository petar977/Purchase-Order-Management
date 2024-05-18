using DataLayer.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class CounterViewModel
    {
        public IEnumerable<Company> CompanyList { get; set; }
        public OrderCountDto OrderCount { get; set; }
    }
}
