namespace TestForInboost.DTO.Weather
{
    public class mainWeatherObjectDTO
    {
        public int id { get; set; }
        public int WeatherHistoryId { get; set; } // forean key for weather

        public decimal temp {  get; set; }
        public decimal feels_like { get; set; }
        public decimal temp_min { get; set; }
        public decimal temp_max { get; set; }

        public int pressure { get; set; }
        public int humidity { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }



    }
}
