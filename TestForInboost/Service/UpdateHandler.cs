using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TestForInboost.DTO;

namespace TestForInboost.Service;

public class UpdateHandler(ITelegramBotClient bot,  ILogger<UpdateHandler> logger, IServiceScopeFactory _serviceScopeFactory, OpenWeatherMapService _openWeatherService) : IUpdateHandler
{
    
    


    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;


        if (messageText.IndexOf("/weather") == 0)
        {
            string city = messageText.Replace("/weather", "").Trim();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dapper = scope.ServiceProvider.GetRequiredService<DapperRepository>();
                var _Weather = scope.ServiceProvider.GetRequiredService<OpenWeatherMapService>();

                var weatherResponse = await _Weather.getWeatherForCity(city);

                if (await _dapper.GetUser(msg.Chat.Id) == null)
                {
                    await _dapper.CreateUser(new UserDTO
                    {
                        Id = msg.From.Id,
                        isBot = msg.From.IsBot,
                        FirstName = msg.From.FirstName,
                        LastName = msg.From.LastName,
                        UserName = msg.From.Username

                    });
                }


                Message sentMessage = new Message();

                if (weatherResponse.GetProperty("cod").GetInt32() == 200)
                {

                    int weatherId = await _dapper.CreateWeatherHistory(weatherResponse, msg.From.Id);
                    sentMessage = await SendWeather(msg, weatherId, city);
                }
                else
                {
                    sentMessage = await SendUnfoundCity(msg, city);

                }



                logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);

            }


        }
        else
        {
            await Usage(msg);
        }
    }

    async Task<Message> SendWeather(Message msg, int weatherHistoryID, string city)
    {
        string serilizeWeather = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(weatherHistoryID)));

        var inlineButton = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Get Weather", serilizeWeather) }
        });

        var message = await bot.SendMessage(
        msg.Chat,
        $"City {city} is found click to button below for recieve weather",
        ParseMode.Html,
        protectContent: true,
        replyMarkup: inlineButton
        );

        return message;
    }

    async Task<Message> SendUnfoundCity(Message msg, string city)
    {
        var message = await bot.SendMessage(
        msg.Chat,
        $"City {city} is not found, try another name "
        );
        return message;
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        int WeatherId = JsonSerializer.Deserialize<int>(Encoding.UTF8.GetString(Convert.FromBase64String(callbackQuery.Data)));

        WeatherHistoryDTO Weather;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _dapper = scope.ServiceProvider.GetRequiredService<DapperRepository>();
            Weather = await _dapper.GetWeatherHistoryById(WeatherId);

        }

        string weatherInfo = $"main: {Weather.Main}\n" +
                             $"description: {Weather.Description}\n" +            
                             $"\n" +
                             $"Temp:°C\n" +
                             $" temp: {Weather.temp - 273}°C\n" +
                             $" feels_like: {Weather.temp_feels_like - 273}°C\n" +
                             $" temp_min: {Weather.temp_min - 273}°C\n" +
                             $" temp_max: {Weather.temp_max - 273}°C\n" +
                             $"\n" +
                             $"Pressure:{Weather.pressure} Pa\n" +
                             $"\n" +
                             $"humidity: {Weather.humidity} %\n" +
                             $"\n" +
                             $"Wind speed: {Weather.wind_speed} m/s\n" +
                             $"Wind deg: {Weather.wind_deg}"+
                             $"\n" +
                             $"Clouds:{Weather.cloud}% \n"+
                             $"\n" +
                             $"Visibility: {Weather.visibility}\n";



        await bot.SendMessage(
            chatId: callbackQuery.Message.Chat.Id,
            text: weatherInfo
            );
    }


    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                /weather {city name} - send wether of city                
            """;
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}