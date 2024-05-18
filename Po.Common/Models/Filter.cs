using DataLayer.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Models
{
    public class Filter
    {
        public int start { get; set; }
        public Column[]? columns { get; set; }
        public Search? search { get; set; }
        public Order[]? order { get; set; }
      
        public int length { get; set; }
        public int draw { get; set; }
        public string? sortCol
        {
            get
            {               
                if (order == null)
                {
                    return string.Empty;
                }
                return columns[order[0].column].data;
            }
        }
        public string orderDir 
        { 
            get 
            {
                if (order == null)
                {
                    return string.Empty;
                }
                return order[0].dir; 
            } 
        }
    }
    public class Column
    {
        public string? data { get; set; }
        public string? name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search? search { get; set; }
    }
    public class Search
    {
        public string? value { get; set; }
        public bool regex { get; set; }
    }
    public class Order
    {
        public string? dir { get; set; }
        public int column { get; set; }
    }
    public class PurchaseOrderFilter : Filter
    {
        public int selectedByDays { get; set; }
        public State selectedByStatus { get; set; }
        public int selectedByCompany{ get; set; }
    }


}
