namespace TimelyTastes.Models;

public class ErrorViewModel
{
    //Handles error Associated with the payment workflow
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
