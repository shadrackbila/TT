using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TimelyTastes.Validation.VerifyDateAndTimeAttribute;

namespace TimelyTastes.Models
{
    public class Listing
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "The listing Vendors ID is required")]
        public string VendorID { get; set; } = "";

        [Required(ErrorMessage = "The item Image is required")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [Required(ErrorMessage = "The product Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "The description is required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "The discount price is required")]
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "The original Price is required")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "The quantity of bags available is required")]
        public int QuantityAvailable { get; set; }

        [Required(ErrorMessage = "The pick up starting time is required")]
        [VerifyDateAndTime("AvailableUntil", ErrorMessage = "End time must be at least 30 minutes after the start time")]
        public DateTime AvailableFrom { get; set; }

        [Required(ErrorMessage = "The pick up expiration time is required")]
        public DateTime AvailableUntil { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        public bool HideListing { get; set; } = false;

    }
}