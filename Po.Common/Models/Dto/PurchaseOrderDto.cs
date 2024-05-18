using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models.Dto
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vendor name is required.")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
        [Required(ErrorMessage = "Payment Type is required.")]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }        
        [Display(Name = "Ordered By")]
        public string? OrderedBy { get; set; }
        [Required(ErrorMessage = "Prior purchase information is required.")]
        [Display(Name = "Prior purchase information")]
        public string? Info { get; set; }
        public bool IsReadOnly { get; set; }
        public List<ManageUserCompaniesViewModel>? Companies { get; set; }
        public List<OrderCountDto>? Counter { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set;}
        [Required(ErrorMessage = "Type is required.")]
        public int CounterId { get; set; }
        public string? PoNumber { get; set; }

    }
}
