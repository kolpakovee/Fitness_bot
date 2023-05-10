using System.Globalization;
using System.Text;
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
                    "🗓️Расписание",
                    "t_timetable")
            },
            // second row
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    " 👥Клиенты",
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
                    "Расписание на 7️⃣ дней",
                    "week_timetable")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "➕Добавить тренировку",
                    "add_training")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "➖Отменить тренировку",
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

                keyboardButtons.Add(InlineKeyboardButton.WithCallbackData(firstDay.Day.ToString(),
                    firstDay.ToString("dd/MM/yyyy")));
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
            string startTime = i > 9 ? i.ToString() : "0" + i;
            string middleTime = i + 1 > 9 ? (i + 1).ToString() : "0" + (i + 1);
            string endTime = i + 2 > 9 ? (i + 2).ToString() : "0" + (i + 2);
            
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"{startTime}:00 - {middleTime}:00", $"{startTime}:00"),
                InlineKeyboardButton.WithCallbackData($"{middleTime}:00 - {endTime}:00", $"{middleTime}:00")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetButtonsFromListOfTrainings(List<Training> trainings, string command)
    {
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

        foreach (var training in trainings)
        {
            string? username = training.ClientUsername == "окно" ? "🖼️ window" : "🪪 " + training.ClientUsername;
            string text = command == "delete"
                ? $"⌚️ {training.Time:dd.MM HH:mm} 📍{training.Location} {username}"
                : $"⌚️ {training.Time:dd.MM HH:mm} 📍{training.Location}";
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(
                    text,
                    $"{command}*{training.Identifier}")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetButtonsFromListOfClients(List<Client> clients, string command)
    {
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

        if (command == "add_for_training")
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("окно", $"{command}*окно")
            });
        }

        if (clients.Count == 0 && command != "add_for_training")
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Пока что здесь пусто :)", "ignore")
            });
            return new InlineKeyboardMarkup(buttons);
        }

        foreach (var client in clients)
        {
            string str = client.Identifier;
            if (client.FinishedForm())
                str = $"{client.Name} {client.Surname}";

            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(str, $"{command}*{client.Identifier}")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetButtonsForClientForm(Client client)
    {
        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData($"рост: {client.Height}", "edit*height"),
                InlineKeyboardButton.WithCallbackData($"вес: {client.Weight}", "edit*weight")
            },

            new()
            {
                InlineKeyboardButton.WithCallbackData($"грудь: {client.Bust}", "edit*bust"),
                InlineKeyboardButton.WithCallbackData($"талия: {client.Waist}", "edit*waist")
            },

            new()
            {
                InlineKeyboardButton.WithCallbackData($"живот: {client.Stomach}", "edit*stomach"),
                InlineKeyboardButton.WithCallbackData($"бёдра: {client.Hips}", "edit*hips")
            },

            new()
            {
                InlineKeyboardButton.WithCallbackData($"нога: {client.Legs}", "edit*legs"),
                InlineKeyboardButton.WithCallbackData($"цель: {client.Goal}", "edit*goal")
            }
        };

        return new InlineKeyboardMarkup(buttons);
    }
}