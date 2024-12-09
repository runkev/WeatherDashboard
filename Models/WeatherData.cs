using Newtonsoft.Json;

namespace WeatherDashboard.Models
{
    // Main Class
    public class WeatherData
    {
        public class Temperature{
            public double Fahrenheit { get; set; }
            public double Celsius { get; set; }
        }
        // JsonProperty attribute is used to map JSON keys to C# properties
        [JsonProperty("main")]
        public MainData Main { get; set; }

        [JsonProperty("name")]
        public string CityName { get; set; }

        [JsonProperty("weather")]
        public List<WeatherCondition> Weather { get; set; }

        // Inner Class
        public class MainData
        {
            private double _temp;

            [JsonProperty("temp")]
            public double Temperature 
            { 
                get => _temp; 
                set => _temp = value; 
            }
            
            [JsonProperty("feels_like")]
            public double FeelsLike { get; set; }

            [JsonProperty("humidity")]
            public int Humidity { get; set; }
        }
        // Inner Class
        public class WeatherCondition
        {
            [JsonProperty("main")]
            public string Main { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}