namespace TestForInboost.DTO.Weather
{
    public class weatherObjectDTO
    {
        public int id { get; set; }
        public int WeatherHistoryId { get; set; } // forean key for weather
        public string main { get; set; }
        public string description { get; set; }
    }
}
