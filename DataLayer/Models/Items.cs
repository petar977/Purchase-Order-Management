using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Items
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double UnitPrice { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Companies { get; set; }
    }
}
