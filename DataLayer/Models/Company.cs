using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Status Status { get; set; }
        public string? Address {  get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }

    }
    public enum Status
    {
        Inactive=0,
        Active=1  
    }
}
