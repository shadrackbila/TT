using TimelyTastes.Models;

namespace TimelyTastes.Services
{
    public interface ISendEmail
    {
        bool SendEmail(Orders order);

        bool RequestRating(Orders order, RatingInvitation invitation);
    }
}