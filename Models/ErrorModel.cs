namespace TimelyTastes.Models
{
    public class ErrorModel
    {
        //Handles error Associated with the login(firebase) workflow

        public int code { get; set; }
        public string message { get; set; } = "";

        public List<ErrorModel>? errors { get; set; }

    }
}