using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class DataTableOrder
    {
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<PurchaseOrderDto> data { get; set; }
    }
}
