using System.ComponentModel.DataAnnotations;

namespace TimelyTastes.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Please enter your Email Adress")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter the passoword")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please confirm the passoword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";



    }
}