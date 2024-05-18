using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class DataTableItems
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<ItemsDto>? Data { get; set; }
    }
}
