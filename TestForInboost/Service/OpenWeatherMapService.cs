using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TestForInboost.DTO;

namespace TestForInboost.Service
{

    public class OpenWeatherMapService(HttpClient _httpClient, IConfiguration _configuration)
    {

        public async Task<JsonElement> getWeatherForCity(string city) 
        {

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_configuration["openweathermapKey"]}";

            var jsonresponce = await _httpClient.GetAsync(url);

            var content = await jsonresponce.Content.ReadAsStringAsync();

            var jsonDocument = JsonDocument.Parse(content);


            var json = jsonDocument.RootElement;


            return json;
        }
    }
}
