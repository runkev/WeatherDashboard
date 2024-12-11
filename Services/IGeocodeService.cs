// Services/IGeocodeService.cs
namespace WeatherDashboard.Services
{
    public interface IGeocodeService
    {
        Task<(double Latitude, double Longitude)> GetCoordinates(string location);
    }
}