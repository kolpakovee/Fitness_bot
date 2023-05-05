using Fitness_bot.Model.BLL;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;

namespace Fitness_bot.TelegramBot;

// TODO: написать комментарии со всеми входными параметрами
// TODO: сделать логгер
// TODO: убрать заморозку из ТЗ!!!!!!!!!

static class TelegramBot
{
    // TODO: спрятать в конфиг
    private static readonly ITelegramBotClient BotClient =
        new TelegramBotClient("5825304594:AAHQgVHuB4bfB01eetUGtXhGudpPMMLiRaI");

    static void Main()
    {
        var cts = new CancellationTokenSource();
        
        // TODO: создать класс инициализации и пихнуть туда
        MessageSender messageSender = new MessageSender(BotClient, cts.Token);
        TelegramBotLogic telegramBotModel = new TelegramBotLogic(messageSender);
        TelegramBotPresenter presenter = new TelegramBotPresenter(telegramBotModel);
        
        // Запуск бота
        BotClient.Start(
            cts.Token,
            presenter.HandleUpdate,
            presenter.HandleError
        );

        Console.ReadLine();
    }
}