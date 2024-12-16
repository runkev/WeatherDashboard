using WeatherDashboard.Models;
using WeatherDashboard.Models.Response;

namespace WeatherDashboard.Services
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        private readonly IGeocodeService _geocodeService;

        public WeatherService(HttpClient client, IGeocodeService geocodeService)
        {
            _client = client;
            var userEmail = Environment.GetEnvironmentVariable("USER_EMAIL");
            _client.DefaultRequestHeaders.Add("User-Agent", $"(WeatherDashboard {userEmail})");
            _geocodeService = geocodeService;
        }

        public async Task<WeatherData> GetWeatherByLocation(string location)
        {
            try 
            {
                Console.WriteLine($"WeatherService - Location searched for: {location}");
                
                var coordinates = await _geocodeService.GetCoordinates(location);
                Console.WriteLine($"Raw coordinates returned: {coordinates}");
                
                var (latitude, longitude) = coordinates;
                Console.WriteLine($"Coordinates after deconstruction: {latitude}, {longitude}");
                
                var pointsUrl = $"https://api.weather.gov/points/{latitude},{longitude}";
                Console.WriteLine($"Requesting NWS API at: {pointsUrl}");
                
                var pointsResponse = await _client.GetAsync(pointsUrl);
                pointsResponse.EnsureSuccessStatusCode();
                
                var pointsData = await pointsResponse.Content.ReadFromJsonAsync<PointsResponse>();
                if (pointsData?.Properties == null)
                {
                    throw new Exception("Points response was null or invalid");
                }
                
                Console.WriteLine($"Forecast URL: {pointsData.Properties.Forecast}");
                Console.WriteLine($"Hourly Forecast URL: {pointsData.Properties.ForecastHourly}");
                
                var currentResponse = await _client.GetAsync(pointsData.Properties.Forecast);
                var hourlyResponse = await _client.GetAsync(pointsData.Properties.ForecastHourly);
                
                currentResponse.EnsureSuccessStatusCode();
                hourlyResponse.EnsureSuccessStatusCode();
                
                var currentForecast = await currentResponse.Content.ReadFromJsonAsync<ForecastResponse>();
                var hourlyForecast = await hourlyResponse.Content.ReadFromJsonAsync<ForecastResponse>();
                
                if (currentForecast?.Properties?.Periods == null || hourlyForecast?.Properties?.Periods == null)
                {
                    throw new Exception("Forecast response was null or invalid");
                }
                
                return new WeatherData
                {
                    Location = location,
                    Current = currentForecast.Properties.Periods[0],
                    DailyForecast = currentForecast.Properties.Periods,
                    HourlyForecast = hourlyForecast.Properties.Periods
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWeatherByLocation: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw the exception after logging
            }
        }
    }
}