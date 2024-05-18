using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace DataLayer.Data
{
    public class PoDbContext : IdentityDbContext<IdentityUser>
    {
        public PoDbContext(DbContextOptions<PoDbContext> options) : base(options)
        {

        }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderItems> PurchaseOrderItem { get; set; }
        public virtual DbSet<Items> Item {  get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<OrderCount> OrderCount { get; set; }
        

    }
}
