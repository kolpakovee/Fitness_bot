using Telegram.Bot.Types.ReplyMarkups;

namespace Fitness_bot.View;

public static class MenuButtons
{
    public static InlineKeyboardMarkup TrainerMenu()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            // first row
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Расписание",
                    "t_timetable")
            },
            // second row
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Клиенты",
                    "clients")
            }
        });
        return inlineKeyboard;
    }

    public static InlineKeyboardMarkup TrainerTimetableMenu()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Расписание на 7 дней",
                    "week_timetable")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Добавить тренировку",
                    "add_training")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Отменить тренировку",
                    "cancel_training")
            }
        });
        return inlineKeyboard;
    }
    
    public static InlineKeyboardMarkup TrainerClientsMenu()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Добавить клиента",
                    "add_client")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Удалить клиента",
                    "delete_client")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Просмотр базы",
                    "check_base")
            }
        });
        return inlineKeyboard;
    }
    
    public static InlineKeyboardMarkup ClientMenu()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Расписание",
                    "cl_timetable")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Тренировки",
                    "cl_trainings")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Анкета",
                    "cl_form")
            }
        });
        return inlineKeyboard;
    }
    
    public static InlineKeyboardMarkup YesOrNoButtons()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Да, я тренер",
                    "i_am_trainer")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Нет, я клиент",
                    "i_am_client")
            }
        });
        return inlineKeyboard;
    }
    
    public static InlineKeyboardMarkup ClientTrainingMenu()
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Записаться на тренировку",
                    "cl_record")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "Отменить тренировку",
                    "cl_cancel")
            }
        });
        return inlineKeyboard;
    }
}