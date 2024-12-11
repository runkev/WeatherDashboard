// Services/GeocodeService.cs
using WeatherDashboard.Models.Response;

public class CensusGeocodeService : IGeocodeService
{
    private readonly HttpClient _client;
    private const string BaseUrl = "https://geocoding.geo.census.gov/geocoder/locations/";

    public CensusGeocodeService(HttpClient client)
    {
        _client = client;
    }

    public async Task<(double Latitude, double Longitude)> GetCoordinates(string location)
    {
        bool isZipCode = location.Length == 5 && location.All(char.IsDigit);
        
        string requestUrl;
        if (isZipCode)
        {
            requestUrl = $"{BaseUrl}address?zip={location}&benchmark=2020&format=json";
        }
        else
        {
            requestUrl = $"{BaseUrl}onelineaddress?address={Uri.EscapeDataString(location)}&benchmark=2020&format=json";
        }

        var response = await _client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<CensusResponse>();

        if (result?.Result?.AddressMatches == null || !result.Result.AddressMatches.Any())
        {
            throw new Exception($"Location '{location}' not found");
        }

        var match = result.Result.AddressMatches.First();
        return (match.Coordinates.Latitude, match.Coordinates.Longitude);
    }
}