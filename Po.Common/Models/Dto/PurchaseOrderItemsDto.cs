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
    public class PurchaseOrderItemsDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Item Name")]
        public string? ItemsName { get; set; }
        [Required(ErrorMessage = "Please enter the Quantity")]
        [Display(Name = "Quatity")]
        public int Qty { get; set; }
        [Required(ErrorMessage = "Please enter the Price")]
        [Display(Name = "Unit Price")]
        public double UnitPrice { get; set; }
        public double? Total { get; set; }
        public string? Link {get; set;}
        public int PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }

    }
}
