using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class Vendors
    {

        // public int ShopId { get; set; } = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorID { get; set; } = 1;

        [Required]
        public byte[] Logo { get; set; } = Array.Empty<byte>();

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Biography { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Required]
        public string ShopOwnerName { get; set; } = "";

        [Required]
        public int FoodQuality { get; set; } = 0;

        [Required]
        public int FoodQuantity { get; set; } = 0;

        [Required]
        public int FoodVariety { get; set; } = 0;

        [Required]
        public int CollectionExperience { get; set; } = 0;

        [Required]
        public double Rating { get; set; } = 0;

        [Required]
        public int SavedMeals { get; set; } = 0;

        [Required]
        public int TotalReviews { get; set; } = 0;

        [NotMapped]
        public IFormFile LogoImageFile { get; set; }

    }
}