using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class JsonResponse
    {
        public string? Message {get; set;}
        public bool Success { get; set;}
        public int Id { get; set;}
        public List<object>? Companies { get; set;}
    }
}
