namespace WeatherDashboard.Models.Response
{
    public class NominatimResponse
    {
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public Address? Address { get; set; }
    }

    public class Address
    {
        public string? Country { get; set; }
    }
}