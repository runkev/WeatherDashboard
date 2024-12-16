namespace WeatherDashboard.Models.Response
{
    public class PointsResponse
    {
        public PointsProperties? Properties { get; set; }
    }

    public class PointsProperties
    {
        public string? Forecast { get; set; }
        public string? ForecastHourly { get; set; }
    }
}