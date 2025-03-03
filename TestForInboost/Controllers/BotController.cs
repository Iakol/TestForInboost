using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using TestForInboost.Service;
using Microsoft.Extensions.Options;
using TestForInboost.DTO;

namespace TestForInboost.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController(IOptions<BotConfiguration> Config, DapperRepository _db) : ControllerBase
    {
        
        [HttpGet("setWebhook")]
        public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
        {
            var webhookUrl = Config.Value.BotWebhookUrl.AbsoluteUri;
            await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
            return $"Webhook set to {webhookUrl}";
        }

        [HttpPost]
        [Route("/")]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
        {
            if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
                return Forbid();
            try
            {
                await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            }
            catch (Exception exception)
            {
                await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/Users/:id")]
        public async Task<ActionResult<UserWithWeatherHistoryDTO>> GetUserInfo(long id)
        {
            return Ok(await _db.GetUserAndHimHistory(id));
        }

        [HttpPost]
        [Route("api/Users/[action]")]

        public async Task<ActionResult> NotifyallOrOneUser([FromServices] ITelegramBotClient bot, string Text, long? id)
        {
            List<UserDTO> users = await _db.GetUsersList();

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
