namespace TimelyTastes.Services
{
    /// <summary>
    /// Service for calculating distances between locations using Radar.io's API.
    /// 
    /// API Reference: https://api.radar.io/v1/route/distance
    /// This service calculates the distance between two geographic points (vendor and user)
    /// using Radar.io's distance calculation API.
    /// </summary>
    /// <remarks>
    /// Required Parameters:
    /// - source (string): Origin coordinates in "latitude,longitude" format
    /// - destination (string): Destination coordinates in "latitude,longitude" format
    /// - modes (string): Travel modes as comma-separated values (car, truck, foot, bike)
    /// 
    /// Optional Parameters:
    /// - units (string): Distance units - "metric" or "imperial". Defaults to imperial.
    /// - avoid (string): Route features to avoid as comma-separated values (tolls, highways, ferries, borderCrossings)
    /// - geometry (string): Response geometry format (linestring, polyline5, polyline6). Defaults to none.
    /// - departureTime (string): ISO 8601 date string for historical traffic calculation
    /// 
    /// Note: Replace the Radar API key with a secure configuration value.
    /// </remarks>
    public class RadarDistanceApi : IRadarDistanceApi
    {
        /// <summary>
        /// Calculates the distance between two geographic coordinates using Radar.io API.
        /// </summary>
        /// <param name="sourceLatitude">Latitude of the origin point</param>
        /// <param name="sourceLongitude">Longitude of the origin point</param>
        /// <param name="destinationLatitude">Latitude of the destination point</param>
        /// <param name="destinationLongitude">Longitude of the destination point</param>
        /// <param name="modes">Travel modes (comma-separated: car, truck, foot, bike)</param>
        /// <param name="units">Distance units (metric or imperial)</param>
        /// <returns>
        /// JSON response string containing distance information.
        /// Returns empty string if the API call fails.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails</exception>
        public async Task<string> GetDistanceAsync(
            string sourceLatitude,
            string sourceLongitude,
            string destinationLatitude,
            string destinationLongitude,
            string modes,
            string units)
        {

            string radarApiKey = "prj_test_pk_569d0e48af27a42ef96769319214f2d1719f5771";
            string returnResponse = "";

            // Construct the Radar API URL with query parameters
            var radarApiUrl = $"https://api.radar.io/v1/route/distance?origin={sourceLatitude},{sourceLongitude}&destination={destinationLatitude},{destinationLongitude}&modes={modes}&units={units}";

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", radarApiKey);

                var response = await httpClient.GetAsync(radarApiUrl);
                response.EnsureSuccessStatusCode();

                // Log successful API call 
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("Radar API call successful.");
                Console.ResetColor();

                returnResponse = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // Log error 
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error calling Radar API: {ex.Message}");
                Console.ResetColor();


            }

            return returnResponse;
        }
    }
}