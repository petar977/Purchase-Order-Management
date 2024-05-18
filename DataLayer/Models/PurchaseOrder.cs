using System.ComponentModel.DataAnnotations.Schema;


namespace DataLayer.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public required string VendorName { get; set; }
        public required string PaymentType { get; set; }      
        public DateTime Date { get; set; }
        public string? ApprovedBy { get; set; }
        public State Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? OrderedBy { get; set; }
        public string? Info { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Companys { get; set; }
        public string? PoNumber { get; set; }
        public int? CounterId { get; set; }

    }
    public enum State
    {
        
        Pending=1,
        Approved,
        Denied,
        Canceled,
        InProgress
    }
}
