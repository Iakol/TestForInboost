using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using Telegram.Bot.Types;
using TestForInboost.DTO;
using TestForInboost.DTO.Weather;
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
            string UserTable = (await _db.QueryAsync<string>(SQLConstants.CheakUserTable)).FirstOrDefault();
            if (UserTable == null) 
            {
                await _db.QueryAsync(SQLConstants.CreateUserTable);
            }

            string WeatherHistoryTable = (await _db.QueryAsync<string>(SQLConstants.CheakWeatherHistoryCheak)).FirstOrDefault();
            if (WeatherHistoryTable == null)
            {
                await _db.QueryAsync(SQLConstants.CreateWeatherHistoryTable);
            }

            string CloudsTable = (await _db.QueryAsync<string>(SQLConstants.CheakCloudsCheak)).FirstOrDefault();
            if (CloudsTable == null)
            {
                await _db.QueryAsync(SQLConstants.CreateCloudsTable);
            }

            string MainTable = (await _db.QueryAsync<string>(SQLConstants.CheakMainCheak)).FirstOrDefault();
            if (MainTable == null)
            {
                await _db.QueryAsync(SQLConstants.CreateMainTable);
            }

            string WindTable = (await _db.QueryAsync<string>(SQLConstants.CheakWindCheak)).FirstOrDefault();
            if (WindTable == null)
            {
                await _db.QueryAsync(SQLConstants.CreateWindTable);
            }

            string WeatherObjectTable = (await _db.QueryAsync<string>(SQLConstants.CheakWeatherObjectCheak)).FirstOrDefault();
            if (WeatherObjectTable == null)
            {
                await _db.QueryAsync(SQLConstants.CreateWeatherObjectTable);
            }            

        }

        //Get Methods

        public async Task<UserWithWeatherHistoryDTO> GetUserAndHimHistory(long UserID) 
        {
            UserWithWeatherHistoryDTO userwithHistiry = new UserWithWeatherHistoryDTO {
                User = await GetUser(UserID),
                WeatherHistory = await GetWeatherHistoryByUserId(UserID)

            };
            return userwithHistiry;
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

        public async Task<List<WeatherHistoryDTO>> GetWeatherHistoryByUserId(long UserID)
        {
            List<WeatherHistoryDTO> weatherHistoryDTOs = (await _db.QueryAsync<WeatherHistoryDTO>(SQLConstants.GetHistoryOfUser, new { UserId = UserID })).ToList();
            foreach (var item in weatherHistoryDTOs)
            {
                item.clouds = (await _db.QueryAsync<cloudsObjectDTO>(SQLConstants.GetcloudsObjectOfHistoryId, new { WeatherHistoryId = item.Id })).FirstOrDefault();
                item.main = (await _db.QueryAsync<mainWeatherObjectDTO>(SQLConstants.GetcloudsObjectOfHistoryId, new { WeatherHistoryId = item.Id })).FirstOrDefault();
                item.weather = (await _db.QueryAsync<weatherObjectDTO>(SQLConstants.GetWeatherObjectOfHistoryId, new { WeatherHistoryId = item.Id })).ToList();
                item.wind = (await _db.QueryAsync<windObjectDTO>(SQLConstants.GetWindObjectOfHistoryId, new { WeatherHistoryId = item.Id })).FirstOrDefault();

            }

            return weatherHistoryDTOs;
        }

        public async Task<WeatherHistoryDTO> GetWeatherHistoryById(int id) 
        {
            WeatherHistoryDTO weatherHistoryDTO = (await _db.QueryAsync<WeatherHistoryDTO>(SQLConstants.GetHistoryOfHistoryId, new { Id = id })).FirstOrDefault();

            weatherHistoryDTO.clouds = (await _db.QueryAsync<cloudsObjectDTO>(SQLConstants.GetcloudsObjectOfHistoryId, new { WeatherHistoryId = weatherHistoryDTO.Id })).FirstOrDefault();
            weatherHistoryDTO.main = (await _db.QueryAsync<mainWeatherObjectDTO>(SQLConstants.GetMainWeatherObjectOfHistoryId, new { WeatherHistoryId = weatherHistoryDTO.Id })).FirstOrDefault();
            weatherHistoryDTO.weather = (await _db.QueryAsync<weatherObjectDTO>(SQLConstants.GetWeatherObjectOfHistoryId, new { WeatherHistoryId = weatherHistoryDTO.Id })).ToList();
            weatherHistoryDTO.wind = (await _db.QueryAsync<windObjectDTO>(SQLConstants.GetWindObjectOfHistoryId, new { WeatherHistoryId = weatherHistoryDTO.Id })).FirstOrDefault();

            return weatherHistoryDTO;
        }

        // Create Methods

        public async Task CreateUser(UserDTO user)
        {

            await _db.QueryAsync(SQLConstants.CreateUser, new { Id = user.Id, isBot = user.isBot, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName });
            
        }

        public async Task<int> CreateWeatherHistory(WeatherHistoryDTO weatherHistory)
        {

            int idOFhistory = await _db.ExecuteScalarAsync<int>(SQLConstants.CreateWeatherHistory,new { visibility = weatherHistory.visibility , UserId = weatherHistory.UserId});
            await CreateCloudsObject(weatherHistory.clouds, idOFhistory);
            await CreateMainObject(weatherHistory.main, idOFhistory);
            await CreateWindObjectDTO(weatherHistory.wind, idOFhistory);
            await CreateWeatherObject(weatherHistory.weather, idOFhistory);

            return idOFhistory;
        }

        public async Task CreateCloudsObject(cloudsObjectDTO clouds,int weatherHistoryId)
        {

            await _db.QueryAsync(SQLConstants.CreateCloudsObject, new { WeatherHistoryId = weatherHistoryId, all = clouds.all });
        
        }
        public async Task CreateMainObject(mainWeatherObjectDTO mainObject, int weatherHistoryId)
        {

            await _db.QueryAsync(SQLConstants.CreateMainObject, new {
                WeatherHistoryId = weatherHistoryId,
                temp = mainObject.temp,
                feels_like = mainObject.feels_like,
                temp_min = mainObject.temp_min,
                temp_max = mainObject.temp_max,
                pressure = mainObject.pressure,
                humidity = mainObject.humidity,
                sea_level = mainObject.sea_level,
                grnd_level = mainObject.grnd_level,

            });
        }

        public async Task CreateWindObjectDTO(windObjectDTO windObject, int weatherHistoryId)
        {

            await _db.QueryAsync(SQLConstants.CreateWindObject, new { WeatherHistoryId = weatherHistoryId, speed = windObject.speed, deg = windObject.deg });
        }

        public async Task CreateWeatherObject(List<weatherObjectDTO> weatherObjects, int weatherHistoryId)
        {
            foreach (var weatherObject in weatherObjects) {
                await _db.QueryAsync(SQLConstants.CreateWeatherObject, new { WeatherHistoryId = weatherHistoryId, main = weatherObject.main, description = weatherObject.description });

            }
        }

        

    }
}
