using System.ComponentModel.DataAnnotations;

namespace TimelyTastes.Models
{
    public class Browse
    {

        public Listing listing { get; set; }
        public Vendors vendors { get; set; }



    }
}