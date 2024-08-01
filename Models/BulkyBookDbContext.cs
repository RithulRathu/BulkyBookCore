using Microsoft.EntityFrameworkCore;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BulkyBook.Models
{
    public class BulkyBookDbContext : IdentityDbContext<IdentityUser>
    {
        public BulkyBookDbContext(DbContextOptions<BulkyBookDbContext> options) : base(options) { }

        public DbSet<RegisterModel> RegisterModels { get; set; }

        public DbSet<CompanyModel> CompanyModels { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<BulkyBook.Models.Product> Product { get; set; } = default!;
        public DbSet<Cart> Carts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
