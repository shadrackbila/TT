using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class Orders
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string OrderNumber { get; set; } = "";
        public string OTP { get; set; } = "";
        public string OrderStatus { get; set; } = "Unknown";

        public Vendors? Vendor { get; set; }
        public Listing? Listing { get; set; }
    }
}