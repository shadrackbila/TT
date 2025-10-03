using TimelyTastes.Data;
using TimelyTastes.Interfaces;
using TimelyTastes.Models;

namespace TimelyTastes.Repositories
{
    public class DBInitializerRepo : IDBInitializer
    {
        public void Initialize(SQLiteDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Listings.Any() || context.Vendors.Any())
            {
                return;
            }

            var listings = new Listing[]
       {
    new Listing
    {
        Id = 1,
        VendorID = 101,
        Name = "Gourmet Pizza",
        Description = "Delicious hand-tossed pizza with fresh toppings.",
        DiscountPrice = 12.99m,
        OriginalPrice = 15.99m,
        QuantityAvailable = 20,
        AvailableFrom = DateTime.Parse("2025-10-01 10:00"),
        AvailableUntil = DateTime.Parse("2025-10-01 22:00"),
        ImageData = Array.Empty<byte>() // default empty image
    },

    new Listing
    {
        Id = 2,
        VendorID = 102,
        Name = "Vegan Salad Bowl",
        Description = "A healthy mix of greens, quinoa, and chickpeas.",
        DiscountPrice = 8.50m,
        OriginalPrice = 10.00m,
        QuantityAvailable = 15,
        AvailableFrom = DateTime.Parse("2025-10-02 09:00"),
        AvailableUntil = DateTime.Parse("2025-10-02 20:00"),
        ImageData = Array.Empty<byte>()
    },

    new Listing
    {
        Id = 3,
        VendorID = 103,
        Name = "Chocolate Cake Slice",
        Description = "Rich, moist chocolate cake with fudge frosting.",
        DiscountPrice = 4.75m,
        OriginalPrice = 5.50m,
        QuantityAvailable = 30,
        AvailableFrom = DateTime.Parse("2025-10-01 08:00"),
        AvailableUntil = DateTime.Parse("2025-10-01 18:00"),
        ImageData = Array.Empty<byte>()
    }
       };




            var vendors = new Vendors[]{

 new Vendors
                {
                    VendorID = 1,
                    Logo = Array.Empty<byte>(), // placeholder
                    Name = "Tasty Bites",
                    Biography = "A small family-owned food shop specializing in local delicacies.",
                    Address = "123 Main Street, Cityville",
                    ShopOwnerName = "John Doe",
                    FoodQuality = 9,
                    FoodQuantity = 8,
                    FoodVariety = 7,
                    CollectionExperience = 8,
                    Rating = 8.5,
                    SavedMeals = 150,
                    TotalReviews = 45
                },
                new Vendors
                {
                    VendorID = 2,
                    Logo = Array.Empty<byte>(),
                    Name = "Savory Delights",
                    Biography = "Known for gourmet snacks and unique flavors.",
                    Address = "456 Oak Avenue, Townsville",
                    ShopOwnerName = "Jane Smith",
                    FoodQuality = 8,
                    FoodQuantity = 9,
                    FoodVariety = 8,
                    CollectionExperience = 9,
                    Rating = 8.8,
                    SavedMeals = 200,
                    TotalReviews = 60
                },
                new Vendors
                {
                    VendorID = 3,
                    Logo = Array.Empty<byte>(),
                    Name = "Healthy Eats",
                    Biography = "Providing nutritious and balanced meals for health-conscious customers.",
                    Address = "789 Pine Road, Villagetown",
                    ShopOwnerName = "Emily Brown",
                    FoodQuality = 9,
                    FoodQuantity = 7,
                    FoodVariety = 8,
                    CollectionExperience = 9,
                    Rating = 9.0,
                    SavedMeals = 180,
                    TotalReviews = 55
                }
};







            foreach (Vendors v in vendors)
            {
                context.Vendors.Add(v);
            }


            foreach (Listing l in listings)
            {
                context.Listings.Add(l);
            }
            context.SaveChanges();
        }
    }
}