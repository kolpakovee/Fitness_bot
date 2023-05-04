using Fitness_bot.Model.BL;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;

namespace Fitness_bot.TelegramBot;

// TODO: пофиксить ПРОСМОТР БАЗЫ тренера (когда клиент не прошёл анкету)
// TODO: написать комментарии со всеми входными параметрами
// TODO: сделать логгер

static class TelegramBot
{
    // TODO: спрятать в конфиг
    private static readonly ITelegramBotClient BotClient =
        new TelegramBotClient("5825304594:AAHQgVHuB4bfB01eetUGtXhGudpPMMLiRaI");

    static void Main()
    {
        // TODO: не передавать его повсюду а сохранить
        var cts = new CancellationTokenSource();
        
        // TODO: создать класс инициализации и пихнуть туда
        MessageSender messageSender = new MessageSender(BotClient, cts.Token);
        TelegramBotModel telegramBotModel = new TelegramBotModel(messageSender);
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