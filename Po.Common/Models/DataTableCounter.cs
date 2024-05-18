using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class DataTableCounter
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<OrderCountDto>? Data { get; set; }
        
    }
}
