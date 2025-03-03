using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Telegram.Bot.Types;
using TestForInboost.DTO;
using TestForInboost.SQL;

namespace TestForInboost.Service
{
    public class DapperRepository
    {
        private readonly IDbConnection _db;

        public DapperRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task InitiateDataBase() 
        {

            if (!_db.ExecuteScalar<bool>(SQLConstants.CheakTable, new { TableName = "Users" }))
            {
                await _db.QueryAsync(SQLConstants.CreateUserTable);
            }
            if (!_db.ExecuteScalar<bool>(SQLConstants.CheakTable, new { TableName = "WeatherHistory" }))
            {
                await _db.QueryAsync(SQLConstants.CreateWeatherHistoryTable);
            }           

        }

        //Get Methods

        public async Task<UserWithWeatherHistoryDTO> GetUserAndHimHistory(long UserID) 
        {
            UserWithWeatherHistoryDTO userWithHistory = new UserWithWeatherHistoryDTO
            {
                User = await GetUser(UserID),
                WeatherHistory = await GetWeatherHistoryByUserId(UserID)
            };
            return userWithHistory; 
        }
        public async Task<UserDTO?> GetUser(long UserID) 
        {
            UserDTO? user = (await _db.QueryAsync<UserDTO>(SQLConstants.GetUser, new { id = UserID })).FirstOrDefault();
            return user;
        }

        public async Task<List<UserDTO>> GetUsersList()
        {
            List<UserDTO> users = (await _db.QueryAsync<UserDTO>(SQLConstants.GetUserList)).ToList();
            return users;
        }

        
        public async Task<WeatherHistoryDTO> GetWeatherHistoryById(int id) 
        {
            return (await _db.QueryAsync<WeatherHistoryDTO>(SQLConstants.GetWeatherHstoryById, new { Id = id })).First();
        }

        public async Task<List<WeatherHistoryDTO>> GetWeatherHistoryByUserId(long id)
        {
            return (await _db.QueryAsync< WeatherHistoryDTO>(SQLConstants.GetHistoryByUserList, new { UserId = id })).OrderByDescending(h=>h.createAt).ToList();
        }

        // Create Methods

        public async Task<int> CreateWeatherHistory(JsonElement json,long UserId)
        {

            int WeatherHistoryId = await _db.ExecuteScalarAsync<int>(SQLConstants.CreateWeatherHistory,
                new
                {
                    UserId,
                    Main = json.GetProperty("weather").EnumerateArray().First().GetProperty("main").GetString(),
                    Description = json.GetProperty("weather").EnumerateArray().First().GetProperty("description").GetString(),
                    temp = json.GetProperty("main").GetProperty("temp").GetDecimal(),
                    temp_feels_like = json.GetProperty("main").GetProperty("feels_like").GetDecimal(),
                    temp_min = json.GetProperty("main").GetProperty("temp_min").GetDecimal(),
                    temp_max = json.GetProperty("main").GetProperty("temp_max").GetDecimal(),
                    pressure = json.GetProperty("main").GetProperty("pressure").GetInt32(),
                    humidity = json.GetProperty("main").GetProperty("humidity").GetInt32(),
                    wind_speed = json.GetProperty("wind").GetProperty("speed").GetDecimal(),
                    wind_deg = json.GetProperty("wind").GetProperty("deg").GetDecimal(),
                    cloud = json.GetProperty("clouds").GetProperty("all").GetInt32(),
                    visibility = json.GetProperty("visibility").GetInt32(),
                    createAt = DateTime.Now
                });

            return WeatherHistoryId;
        }


        public async Task CreateUser(UserDTO user)
        {

            await _db.QueryAsync(SQLConstants.CreateUser, new { Id = user.Id, isBot = user.isBot, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName });
            
        }     

    }
}
