using WeatherDashboard.Models.Response;
using WeatherDashboard.Services;

public class NominatimGeocodeService : IGeocodeService
{
    private readonly HttpClient _client;

    public NominatimGeocodeService(HttpClient client)
    {
        _client = client;
        _client.DefaultRequestHeaders.Add("User-Agent", "(WeatherDashboard kpetow@gmail.com)");
    }

    public async Task<(double Latitude, double Longitude)> GetCoordinates(string location)
    {
        var requestUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(location)}&format=json&limit=1";
        var response = await _client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        
        var results = await response.Content.ReadFromJsonAsync<NominatimResponse[]>();
        
        if (results == null || !results.Any())
            throw new Exception($"Location '{location}' not found");

        return (double.Parse(results[0].Lat), double.Parse(results[0].Lon));
    }
}