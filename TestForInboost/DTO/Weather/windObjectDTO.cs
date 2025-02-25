namespace TestForInboost.DTO.Weather
{
    public class windObjectDTO
    {
        public int id { get; set; }
        public int WeatherHistoryId { get; set; } // forean key for weather
        public decimal speed { get; set; }
        public int deg { get; set; }
    }
}
