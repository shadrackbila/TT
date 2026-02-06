using TimelyTastes.Data;
using Microsoft.EntityFrameworkCore;
using TimelyTastes.Models;

namespace TimelyTastes.Tests.Unit.Helper
{

    public static class TestHelper
    {
        public static SQLiteDbContext GetInMemoryDbContext()
        {
            // Create a new in-memory database context for testing
            var options = new DbContextOptionsBuilder<SQLiteDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new SQLiteDbContext(options);
            context.Database.EnsureCreated();

            SeedVendorTestData(context);

            return context;

        }

        public static void SeedVendorTestData(SQLiteDbContext context)
        {
            // Add test data to the in-memory database
            context.Vendors.Add(new Vendors
            {
                ID = new Guid("11111111-1111-1111-1111-111111111111"),
                VendorID = "testvendor",
                Name = "Test Vendor",
                //logo
                Biography = "This is a test vendor.",
                Address = "123 Test Street",
                ShopOwnerName = "Test Owner",
                FoodQuality = 4.5,
                FoodQuantity = 4.0,
                FoodVariety = 4.2,
                CollectionExperience = 4.8,
                Rating = 4.5,
                SavedMeals = 10,
                TotalReviews = 20

            });
            context.SaveChanges();
        }
    }
}