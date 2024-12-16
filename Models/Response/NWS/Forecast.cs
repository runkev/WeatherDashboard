namespace WeatherDashboard.Models.Response
{
    public class ForecastResponse
    {
        public ForecastProperties? Properties { get; set; }
    }

    public class ForecastProperties
    {
        public List<Period>? Periods { get; set; }
    }
}