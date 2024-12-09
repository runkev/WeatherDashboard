using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Services;
using WeatherDashboard.Models;

namespace WeatherDashboard.Controllers
{
    public class WeatherController : Controller
    {
        // field to store WeatherService instance
        private readonly WeatherService _weatherService; // underscore prefix private fields; readonly can't be changed

        // Constructor to inject WeatherService
        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // shows the main page, no data passed yet
        public IActionResult Index()
        {
            return View(); // returns default view template with no data
        }

        // Take city name parameter and pass it to the service, then return the view with the data
        // run asynchronously to avoid blocking while waiting for API
        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrEmpty(city)) // if empty, return the default view
                return View("Index");

            var (weatherData, fahrenheit, celsius, feelsLikeF, feelsLikeC) = await _weatherService.GetTemperatures(city); 
            ViewBag.Fahrenheit = fahrenheit;
            ViewBag.Celsius = celsius;
            ViewBag.FeelsLikeF = feelsLikeF;
            ViewBag.FeelsLikeC = feelsLikeC;

            return View("Index", weatherData); // return the view with the data
        }
    }
}