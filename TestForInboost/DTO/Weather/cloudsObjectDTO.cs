namespace TestForInboost.DTO.Weather
{
    public class cloudsObjectDTO
    {
        public int id { get; set; }
        public int WeatherHistoryId { get; set; } // forean key for weather
        public int all { get; set; }
    }
}
