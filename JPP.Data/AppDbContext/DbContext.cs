using JPP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JPP.Data.AppDbContext
{
    public class AppDbContext : DbContext
    {
        // Ubah DbContextOptions<DbContext> menjadi DbContextOptions<AppDbContext>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Pastikan menggunakan BIZCustomer, bukan Customer
        public DbSet<BIZCustomer> Customers { get; set; }
    }
}