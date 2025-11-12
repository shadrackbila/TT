using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class Listing
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Listing VendorID is required")]
        public string VendorID { get; set; } = "";

        [Required(ErrorMessage = "The Item Image is required")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [Required(ErrorMessage = "The Product Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "The Description is required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "The Discount price is required")]
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "The Original Price is required")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "The Quantity of bags available is required")]
        public int QuantityAvailable { get; set; }

        [Required(ErrorMessage = "The the Pick up starting time is required")]
        public DateTime AvailableFrom { get; set; }

        [Required(ErrorMessage = "The the Pick up expiration time is required")]
        public DateTime AvailableUntil { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}