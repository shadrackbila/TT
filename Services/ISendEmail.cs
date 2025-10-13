using TimelyTastes.Models;

namespace TimelyTastes.Services
{
    public interface ISendEmail
    {
        bool SendEmail(Orders order);
    }
}