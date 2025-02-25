using TestForInboost.DTO.Weather;

namespace TestForInboost.DTO
{
    public class WeatherHistoryDTO
    {
        public int Id { get; set; }
        public long UserId { get; set; } // forean key to User
        public List<weatherObjectDTO> weather { get; set; }
        public mainWeatherObjectDTO main { get; set; }

        public int visibility { get; set; }

        public windObjectDTO wind { get; set; }

        public cloudsObjectDTO clouds { get; set; }

        public int cod { get; set; } = 200;

    }
}
