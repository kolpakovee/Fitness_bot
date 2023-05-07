using System.Globalization;
using Fitness_bot.Model.Domain;
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

    public static InlineKeyboardMarkup GetCalendarButtons()
    {
        DateTime date = DateTime.Now;
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData(date.ToString("MMMM", new CultureInfo("ru-RU")))
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("пн", "ignore"),
                InlineKeyboardButton.WithCallbackData("вт", "ignore"),
                InlineKeyboardButton.WithCallbackData("ср", "ignore"),
                InlineKeyboardButton.WithCallbackData("чт", "ignore"),
                InlineKeyboardButton.WithCallbackData("пт", "ignore"),
                InlineKeyboardButton.WithCallbackData("сб", "ignore"),
                InlineKeyboardButton.WithCallbackData("вс", "ignore")
            }
        };

        DateTime firstDay = new DateTime(date.Year, date.Month, 1);
        DateTime lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

        var lastMonday = firstDay.DayOfWeek - DayOfWeek.Monday;

        for (int i = 0; i < 5; i++)
        {
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();

            for (int j = 0; j < 7; j++)
            {
                if (i == 0 && lastMonday > 0)
                {
                    keyboardButtons.Add(InlineKeyboardButton.WithCallbackData(" ", "ignore"));
                    lastMonday--;
                    continue;
                }

                if (firstDay > lastDay)
                {
                    keyboardButtons.Add(InlineKeyboardButton.WithCallbackData(" ", "ignore"));
                    continue;
                }

                keyboardButtons.Add(InlineKeyboardButton.WithCallbackData(firstDay.Day.ToString(), firstDay.ToString("dd/MM/yyyy")));
                firstDay = firstDay.AddDays(1);
            }

            buttons.Add(keyboardButtons);
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetTimeIntervals()
    {
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

        for (int i = 8; i < 22; i += 2)
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"{i}:00 - {i + 1}:00", $"{i}:00"),
                InlineKeyboardButton.WithCallbackData($"{i + 1}:00 - {i + 2}:00", $"{i + 1}:00")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetButtonsFromListOfTrainings(List<Training> trainings)
    {
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

        foreach (var training in trainings)
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"{training.Time:dd.MM HH:mm} {training.Location} @{training.ClientUsername}", $"delete*{training.Identifier}")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }
}