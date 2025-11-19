using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class Vendors
    {

        // public int ShopId { get; set; } = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public string VendorID { get; set; } = "";

        [Required(ErrorMessage = "Please upload a logo for your shop.")]
        public byte[] Logo { get; set; } = Array.Empty<byte>();

        [Required(ErrorMessage = "Please enter your shop name.")]
        [StringLength(100, ErrorMessage = "Shop name cannot exceed 100 characters.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Please provide a short biography for your shop.")]
        [StringLength(500, ErrorMessage = "Biography cannot exceed 500 characters.")]
        public string Biography { get; set; } = "";

        [Required(ErrorMessage = "Please enter your shop address.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Please enter the shop owner's name.")]
        public string ShopOwnerName { get; set; } = "";

        [NotMapped]
        public IFormFile? LogoImageFile { get; set; }


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





    }
}