
namespace Po.Common.Models.Dto
{
    public class OrderCountDto
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int CompanyId { get; set; }
        public string FirstLetter { get; set; }
        public string FullNameLetter { get; set; }
        public string CompanyName {  get; set; }
        public int Year { get; set; }
    }
}
