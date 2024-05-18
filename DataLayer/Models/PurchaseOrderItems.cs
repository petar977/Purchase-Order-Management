using System.ComponentModel.DataAnnotations.Schema;


namespace DataLayer.Models
{
    public class PurchaseOrderItems
    {
        public int Id { get; set; }
        public string? ItemsName { get; set; }
        public int Qty { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; } 
        public string? Link { get; set; }
        public int PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder {  get; set; }
    }
}
