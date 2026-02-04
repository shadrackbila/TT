namespace TimelyTastes.Helper
{
    /// Model to hold user location data
    /// <summary>
    /// Represents the geographic location of a user with latitude and longitude.
    /// </summary>

    public class UserLocation
    {
        /// <param name="Latitude">The latitude coordinate of the user's location.</param>
        /// <param name="Longitude">The longitude coordinate of the user's location.</param>

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}