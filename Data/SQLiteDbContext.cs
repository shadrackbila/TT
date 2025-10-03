using Microsoft.EntityFrameworkCore;
using TimelyTastes.Models;

namespace TimelyTastes.Data
{
    public class SQLiteDbContext : DbContext
    {
        public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options) : base(options)
        {

        }

        public DbSet<Listing>? Listings { get; set; }
        public DbSet<Vendors>? Vendors { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Listing>().ToTable("Listing");
            modelBuilder.Entity<Vendors>().ToTable("Vendor");

        }
    }
}