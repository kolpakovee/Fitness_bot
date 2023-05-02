using Fitness_bot.Model.BL;
using Fitness_bot.Model.DAL;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;

namespace Fitness_bot.TelegramBot;

// TODO: написать комментарии со всеми входными параметрами
// TODO: сделать логгер

static class TelegramBot
{
    // TODO: спрятать в конфиг
    private static readonly ITelegramBotClient BotClient =
        new TelegramBotClient("5825304594:AAHQgVHuB4bfB01eetUGtXhGudpPMMLiRaI");

    static void Main()
    {
        var cts = new CancellationTokenSource();

        DataBase db = new DataBase();
        MessageSender messageSender = new MessageSender();
        TelegramBotModel telegramBotModel = new TelegramBotModel(messageSender, db.Connection);
        
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