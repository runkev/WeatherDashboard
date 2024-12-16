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
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        var isZipCode = ZipCodePattern.IsMatch(location);
        Console.WriteLine($"Zip code used? {isZipCode}");
        
        // If it's a ZIP code, append "USA" and use country code filtering
        var searchQuery = isZipCode ? $"{location}, United States" : location;
        
        var requestUrl = $"https://nominatim.openstreetmap.org/search";
        var queryParams = new Dictionary<string, string>
        {
            ["format"] = "json",
            ["addressdetails"] = "1",
            ["limit"] = "1"
        };

        if (isZipCode)
        {
            queryParams["postalcode"] = location;
            queryParams["country"] = "us";
        }
        else
        {
            queryParams["city"] = location;
            queryParams["country"] = "us";
        }

        var queryString = string.Join("&", queryParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        requestUrl = $"{requestUrl}?{queryString}";

        Console.WriteLine($"Request URL: {requestUrl}");

        if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
        }

        var response = await _client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<NominatimResponse[]>();
        if (results == null || results.Length == 0)
            throw new Exception($"Location '{location}' not found");
            
        var result = results[0]; // Store first result to avoid multiple array access
        Console.WriteLine($"Results: Lat={result.Lat}, Lon={result.Lon}, Country={result.Address?.Country}");

        // For ZIP codes, verify the result is actually in the US
        if (isZipCode && result.Address?.Country?.ToLower() != "united states")
        {
            throw new Exception($"ZIP code '{location}' not found in the United States");
        }

        if (result.Lat == null || result.Lon == null)
        {
            throw new Exception("Invalid response from geocoding service: coordinates are null");
        }

        var lat = Math.Round(double.Parse(result.Lat), 4);
        var lon = Math.Round(double.Parse(result.Lon), 4);

        Console.WriteLine($"Formatted coordinates for NWS API: {lat}, {lon}");
    
        return (lat, lon);
    }
}