namespace WeatherDashboard.Models.Response
{
    public class Period
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Name { get; set; }
        public int Temperature { get; set; }
        public string? TemperatureUnit { get; set; }
        public string? ShortForecast { get; set; }
        public string? DetailedForecast { get; set; }
    }
}