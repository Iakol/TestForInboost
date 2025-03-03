
namespace TestForInboost.DTO
{
    public class WeatherHistoryDTO
    {
        public int Id { get; set; }
        public long UserId { get; set; } // forean key to User
        public string Main { get; set; }
        public string Description { get; set; }
        public decimal temp { get; set; }
        public decimal temp_feels_like { get; set; }
        public decimal temp_min { get; set; }
        public decimal temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public decimal wind_speed { get; set; }
        public int wind_deg { get; set; }
        public int cloud { get; set; }
        public int visibility { get; set; }

        public DateTime createAt { get; set; }

    }
}
