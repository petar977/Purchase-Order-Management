using System.ComponentModel.DataAnnotations;

namespace Po.Common.Models.Dto
{
    public class ItemsDto
    {
        public int Id { get; set; }
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        [Display(Name = "Unit Price")]
        public double UnitPrice {  get; set; }
        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        public string? CompanyName {  get; set; }
        public IEnumerable<ManageUserCompaniesViewModel>? Companys { get; set;}

    }
}
