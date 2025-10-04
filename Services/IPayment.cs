using TimelyTastes.Models;

namespace TimelyTastes.Services
{
    public interface IPayment
    {
        string ToUrlEncodedString(Dictionary<string, string> request);
        Dictionary<string, string> ToStringResponse(string response);
        bool AddTransaction(Dictionary<string, string> request, string payrequestId);
        bool UpdateTransaction(Dictionary<string, string> request, string payrequestId);
        Transaction GetTransaction(string payrequestId);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        // ApplicationUser GetAuthonticatedUser();



    }
}