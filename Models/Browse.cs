using System.ComponentModel.DataAnnotations;

namespace TimelyTastes.Models
{
    public class Browse
    {

        public Listing listing { get; set; } = new Listing();
        public Vendors vendors { get; set; } = new Vendors();

    }
}