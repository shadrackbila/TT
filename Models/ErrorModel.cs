namespace TimelyTastes.Models
{
    public class FirebaseErrorWrapper
    {
        public ErrorModel error { get; set; } = new ErrorModel();
    }

    public class ErrorModel
    {
        public int code { get; set; }
        public string message { get; set; } = "";
        public List<ErrorDetail>? errors { get; set; }
    }

    public class ErrorDetail
    {
        public string message { get; set; } = "";
        public string domain { get; set; } = "";
        public string reason { get; set; } = "";
    }

}