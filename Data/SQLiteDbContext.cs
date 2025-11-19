using Microsoft.EntityFrameworkCore;
using TimelyTastes.Models;

namespace TimelyTastes.Data
{
    public class SQLiteDbContext : DbContext
    {
        public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options) : base(options)
        {

        }

        public DbSet<Listing> Listings { get; set; } = default!;
        public DbSet<Vendors> Vendors { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
        public DbSet<Orders> Orders { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Listing>().ToTable("Listing");
            modelBuilder.Entity<Vendors>().ToTable("Vendor");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            modelBuilder.Entity<Orders>().ToTable("Orders");

        }
    }
}