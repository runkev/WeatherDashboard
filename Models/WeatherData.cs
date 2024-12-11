
namespace WeatherDashboard.Models
{
    public class WeatherData
    {
        public Period Current { get; set; }
        public List<Period> DailyForecast { get; set; }
        public List<Period> HourlyForecast { get; set; }
    }
}