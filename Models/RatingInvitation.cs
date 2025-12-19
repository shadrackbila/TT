using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class RatingInvitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid VendorId { get; set; }

        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString(); // Unique token for the link

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30); // Link expires in 30 days
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }

        // Rating values (if you want to store them here)
        public double? FoodQualityRating { get; set; }
        public double? FoodQuantityRating { get; set; }
        public double? FoodVarietyRating { get; set; }
        public double? CollectionExperienceRating { get; set; }
        public string? Comment { get; set; }
    }
}