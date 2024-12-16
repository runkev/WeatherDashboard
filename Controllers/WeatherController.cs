using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Services;

namespace WeatherDashboard.Controllers
{
    public class WeatherController(WeatherService weatherService) : Controller
    {
        // field to store WeatherService instance
        private readonly WeatherService _weatherService = weatherService; // underscore prefix private fields; readonly can't be changed


        // shows the main page, no data passed yet
        public IActionResult Index()
        {
            return View(); // returns default view template with no data
        }

        // Take city name or zip code parameter and pass it to the service, then return the view with the data
        // run asynchronously to avoid blocking while waiting for API
        public async Task<IActionResult> GetWeather(string location)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherByLocation(location);
                return View("Index", weatherData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error getting weather data: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}