namespace TestForInboost.DTO
{
    public class UserWithWeatherHistoryDTO
    {
        public UserDTO User { get; set; }
        public List<WeatherHistoryDTO> WeatherHistory { get; set; }
    }
}
