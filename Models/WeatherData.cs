using WeatherDashboard.Models.Response;

namespace WeatherDashboard.Models
{
    public class WeatherData
    {
        public required string Location { get; set; }
        public Period? Current { get; set; }
        public List<Period>? DailyForecast { get; set; }
        public List<Period>? HourlyForecast { get; set; }
    }
}