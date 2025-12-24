namespace TimelyTastes.Services
{
    public interface IRadarDistanceApi
    {
        Task<string> GetDistanceAsync(string sourceLatitude, string sourceLongitude, string destinationLatitude, string destinationLongitude, string modes, string units);
    }
}