using Fitness_bot.Model.BLL;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Fitness_bot.TelegramBot;

public static class Startup
{
    private static IConfiguration _configuration;

    static Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true);

        _configuration = builder.Build();
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        // регистрация конфигурации приложения с помощью файла appsetings
        services.Configure<Settings>(_configuration.GetSection("Settings"));
        return services;
    }

    public static TelegramBotPresenter InitBot(ITelegramBotClient botClient, CancellationTokenSource cts)
    {
        MessageSender messageSender = new MessageSender(botClient, cts.Token);
        TelegramBotLogic telegramBotModel = new TelegramBotLogic(messageSender);
        return new TelegramBotPresenter(telegramBotModel);
    }
}