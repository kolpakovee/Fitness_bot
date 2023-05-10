using Fitness_bot.Presenter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Fitness_bot.TelegramBot;

// TODO: уведомления о тренировке

static class TelegramBot
{
    public static void Main()
    {
        var cts = new CancellationTokenSource();

        IServiceCollection collection = new ServiceCollection();
        collection.ConfigureServices();
        var settings = collection.BuildServiceProvider().GetRequiredService<IOptions<Settings>>();

        ITelegramBotClient botClient = new TelegramBotClient(settings.Value.Token!);
        TelegramBotPresenter presenter = Startup.InitBot(botClient, cts);
        
        // Запуск бота
        botClient.Start(
            cts.Token,
            presenter.HandleUpdate,
            presenter.HandleError
        );

        Console.ReadLine();
    }
}