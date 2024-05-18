using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class OrderCount
    {
        public int Id { get; set; }       
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public int Count { get; set; }
        public string FirstLetter { get; set; }
        public string FullNameLetter { get; set; }
        public int Year { get; set; }
        
    }
}
