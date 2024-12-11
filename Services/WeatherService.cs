using WeatherDashboard.Models;

namespace WeatherDashboard.Services
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        private readonly IGeocodeService _geocodeService;

        public WeatherService (HttpClient client, IGeocodeService geocodeService)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "WeatherDashboard, kpetow@gmail.com");
            _geocodeService = geocodeService;
        }
    }

    public async Task<WeatherData> GetWeatherByLocation(string location)
    {
        var (latitude, longitude) = await _geocodeService.GetCoordinates(location);

        var pointsUrl = $"https://api.weather.gov/points/{latitude},{longitude}";
        var pointsResponse = await _client.GetAsync(pointsUrl);
        var pointsData = await pointsResponse.Content.ReadFromJsonAsync<Points>();

        var currentResponse = await _client.GetAsync(pointsData.Properties.Forecast);
        var hourlyResponse = await _client.GetAsync(pointsData.Properties.ForecastHourly);

        var currentForecast = await currentResponse.Content.ReadFromJsonAsync<Forecast>();
        var hourlyForecast = await hourlyResponse.Content.ReadFromJsonAsync<Forecast>();

        return new WeatherData
        {
            Current = currentForecast.Properties.Periods[0],
            DailyForecast = currentForecast.Properties.Periods,
            HourlyForecast = hourlyForecast.Properties.Periods
        };
    }
}