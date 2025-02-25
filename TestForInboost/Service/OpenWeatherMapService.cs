using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TestForInboost.DTO;

namespace TestForInboost.Service
{

    public class OpenWeatherMapService 
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration = null)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherHistoryDTO> getWeatherForCity(string city) 
        {
            WeatherHistoryDTO weather = new WeatherHistoryDTO();

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_configuration["openweathermapKey"]}";

            var jsonresponce = await _httpClient.GetAsync(url);

            var content = await jsonresponce.Content.ReadAsStringAsync();

            weather = JsonSerializer.Deserialize<WeatherHistoryDTO>(content);

            return weather;
        }
    }
}
