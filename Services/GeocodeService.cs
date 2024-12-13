using WeatherDashboard.Models.Response;
using WeatherDashboard.Services;
using System.Text.RegularExpressions;

public class NominatimGeocodeService : IGeocodeService
{
    private readonly HttpClient _client;
    private static readonly Regex ZipCodePattern = new Regex(@"^\d{5}$");

    public NominatimGeocodeService(HttpClient client)
    {
        _client = client;
        var userEmail = Environment.GetEnvironmentVariable("USER_EMAIL");
        _client.DefaultRequestHeaders.Add("User-Agent", $"(WeatherDashboard {userEmail})");
    }

    public async Task<(double Latitude, double Longitude)> GetCoordinates(string location)
    {
        var isZipCode = ZipCodePattern.IsMatch(location);
        Console.WriteLine($"Zip code used? {isZipCode}");
        
        // If it's a ZIP code, append "USA" and use country code filtering
        var searchQuery = isZipCode ? $"{location}, United States" : location;
        
        var requestUrl = $"https://nominatim.openstreetmap.org/search" +
            $"?q={Uri.EscapeDataString(searchQuery)}" +
            $"&format=json" +
            $"&addressdetails=1" +  // Get full address details
            (isZipCode ? "&countrycodes=us" : "") +  // Only apply country filter for ZIP codes
            "&limit=1";

            Console.WriteLine($"Request URL: {requestUrl}");

        var response = await _client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        
        var results = await response.Content.ReadFromJsonAsync<NominatimResponse[]>();
        
        if (results == null || !results.Any())
            throw new Exception($"Location '{location}' not found");

        // For ZIP codes, verify the result is actually in the US
        if (isZipCode && 
            results[0].Address?.CountryCode?.ToLower() != "us")
        {
            throw new Exception($"ZIP code '{location}' not found in the United States");
        }

        return (double.Parse(results[0].Lat), double.Parse(results[0].Lon));
    }
}