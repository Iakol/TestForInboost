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

public class UpdateHandler : IUpdateHandler
{
    private static readonly InputPollOption[] PollOptions = ["Hello", "World!"];
    private readonly OpenWeatherMapService _openWeatherService;
    private readonly ITelegramBotClient bot;
    private readonly ILogger<UpdateHandler> logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public UpdateHandler(IServiceScopeFactory serviceScopeFactory, ITelegramBotClient _bot, ILogger<UpdateHandler> _logger, OpenWeatherMapService openWeatherService) 
    {
        _openWeatherService = openWeatherService;
        bot = _bot;
        logger = _logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
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

                WeatherHistoryDTO weatherResponse = await _Weather.getWeatherForCity(city);

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

                weatherResponse.UserId = msg.From.Id;

                Message sentMessage = new Message();


                if (weatherResponse.cod == 200)
                {
                    weatherResponse.Id = await _dapper.CreateWeatherHistory(weatherResponse);
                    sentMessage = await SendWeather(msg, weatherResponse, city);
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

    async Task<Message> SendWeather(Message msg, WeatherHistoryDTO weatherHistory, string city)
    {
        string serilizeWeather = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(weatherHistory.Id)));

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

        string weatherInfo = $"main: {Weather.weather[0].main}\n" +
                             $"description: {Weather.weather[0].description}\n" +            
                             $"\n" +
                             $"Temp:°C\n" +
                             $" temp: {Weather.main.temp - 273}°C\n" +
                             $" feels_like: {Weather.main.feels_like - 273}°C\n" +
                             $" temp_min: {Weather.main.temp_min - 273}°C\n" +
                             $" temp_max: {Weather.main.temp_max - 273}°C\n" +
                             $"\n" +
                             $"Pressure:{Weather.main.pressure} Pa\n" +
                             $"\n" +
                             $"humidity: {Weather.main.humidity} %\n" +
                             $"\n" +
                             $"Wind speed: {Weather.wind.speed} m/s\n" +
                             $"Wind deg: {Weather.wind.deg}"+
                             $"\n" +
                             $"Clouds:{Weather.clouds.all}% \n";



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