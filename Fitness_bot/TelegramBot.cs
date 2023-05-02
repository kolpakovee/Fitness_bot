using Fitness_bot.Presenter;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Fitness_bot;

// TODO: написать комментарии со всеми входными параметрами
// TODO: сделать логгер

static class TelegramBot
{
    // Объект бота
    // TODO: спрятать в конфиг
    private static readonly ITelegramBotClient BotClient =
        new TelegramBotClient("5825304594:AAHQgVHuB4bfB01eetUGtXhGudpPMMLiRaI");

    static void Main()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var receiverOptions = new ReceiverOptions();

        // Запуск бота
        
        // TODO: сервис провайдер из него достать экземпляр TelegramBotPresenter и от него вызвать 
        
        BotClient.StartReceiving(
            TelegramBotPresenter.HandleUpdate,
            TelegramBotPresenter.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.ReadLine();
    }
}