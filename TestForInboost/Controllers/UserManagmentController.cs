using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using Telegram.Bot;
using TestForInboost.DTO;
using TestForInboost.Service;

namespace TestForInboost.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserManagmentController : ControllerBase
    {
        private readonly DapperRepository _db;
        private readonly IOptions<BotConfiguration> Config;


        public UserManagmentController(DapperRepository db, IOptions<BotConfiguration> _Config) 
        {
            _db = db;
            Config = _Config;

        }

        [HttpGet]
        [Route("api/Users/:id")] 
        public async Task<ActionResult<UserWithWeatherHistoryDTO>> GetUserInfo(long id) 
        {
            UserWithWeatherHistoryDTO userAndHistory = new UserWithWeatherHistoryDTO
            {
                User = await _db.GetUser(id),
                WeatherHistory = await _db.GetWeatherHistoryByUserId(id)

            };
            return Ok(userAndHistory);
        }

        [HttpPost]
        [Route("api/Users/[action]")] 

        public async Task<ActionResult> NotifyallOrOneUser ([FromServices] ITelegramBotClient bot,string Text,long? id)
        {
            List<UserDTO> users =await  _db.GetUsersList();

            if (id.HasValue)
            {
                await bot.SendMessage(id.Value, Text);

            }
            else 
            {
            
                foreach (var item in users) 
                {
                    await bot.SendMessage(item.Id, Text);
                }
            }
            return Ok();
        }
    }
}
