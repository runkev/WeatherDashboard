using System.Net.Http;
using Newtonsoft.Json;
using WeatherDashboard.Models;

namespace WeatherDashboard.Services
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5";

        public WeatherService(string apiKey)
        {
            _client = new HttpClient();
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey), "OpenWeatherMap API key is required");
        }

        public async Task<(WeatherData Data, double Far, double Cel, double FarFeelsLike, double CelFeelsLike )> GetTemperatures(string city)
        {
            // Builds URL with your parameters
            var responseImperial = await _client.GetAsync($"{_baseUrl}/weather?q={city}&appid={_apiKey}&units=imperial");
            var dataImperial = JsonConvert.DeserializeObject<WeatherData>(
                await responseImperial.Content.ReadAsStringAsync());
            
            var responseMetric = await _client.GetAsync($"{_baseUrl}/weather?q={city}&appid={_apiKey}&units=metric");
            var dataMetric = JsonConvert.DeserializeObject<WeatherData>(
                await responseMetric.Content.ReadAsStringAsync());

            return (
                dataImperial, 
                dataImperial.Main.Temperature, 
                dataMetric.Main.Temperature, 
                dataImperial.Main.FeelsLike, 
                dataMetric.Main.FeelsLike
            );            
        }
    }
}