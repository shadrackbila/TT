using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelyTastes.Models
{
    public class Orders
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public DateTime OrderDate { get; set; } = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "South Africa Standard Time");

        public string OrderNumber { get; set; } = "";
        public int PinTrys { get; set; } = 0;
        public string OTP { get; set; } = "";
        public string OrderStatus { get; set; } = "Unknown";

        public Vendors? Vendor { get; set; }
        public Listing? Listing { get; set; }
        public string? Email { get; set; }

    }
}