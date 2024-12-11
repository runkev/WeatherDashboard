namespace WeatherDashboard.Models.Response
{
    public class CensusResponse
    {
        public Result Result { get; set; }
    }

    public class Result
    {
        public List<AddressMatch> AddressMatches { get; set; }
    }

    public class AddressMatch
    {
        public Coordinates Coordinates { get; set; }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}