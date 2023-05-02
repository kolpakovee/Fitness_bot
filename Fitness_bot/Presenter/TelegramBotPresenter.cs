using Fitness_bot.Enums;
using Fitness_bot.Model;
using Fitness_bot.Model.BL;
using Fitness_bot.View;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Fitness_bot.Model.User;

namespace Fitness_bot.Presenter;

public class TelegramBotPresenter
{
    public static readonly Dictionary<long, FormStatus> Statuses = new();
    public static readonly Dictionary<long, User> Users = new();
    public static readonly Dictionary<long, ActionStatus> TrainersActions = new();
    public static readonly Dictionary<long, Training> Trainings = new();

    public static async Task HandleUpdate(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
            HandleCallbackQuery(botClient, update, cancellationToken);

        // Если получили сообщение
        if (update.Type == UpdateType.Message)
            HandleMessage(botClient, update, cancellationToken);
    }

    private static void HandleMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;

        // Проверили на null, чтобы дальше не было проблем
        if (message == null || message.Text == null) return;

        long id = message.Chat.Id;

        // Проверили, заполняет ли пользователь анкету
        if (Statuses.ContainsKey(id))
        {
            HandleUserAnswersAsync(botClient, message, cancellationToken);
            return;
        }

        // Проверяем, тренер ли это
        Trainer? trainer = Db.GetTrainerById(id);

        if (trainer != null) // пришёл зарегистрированный тренер
        {
            // если тренер 
            if (TrainersActions.ContainsKey(id))
            {
                HandleTrainerAnswersAsync(botClient, message, cancellationToken);
                return;
            }

            MessageSender.SendTrainerMenu(botClient, message, cancellationToken);
            return;
        }

        if (message.Chat.Username != null)
        {
            User? user = Db.GetUserByUsername(message.Chat.Username);

            // Если в БД нет такого пользователя
            if (user == null)
            {
                MessageSender.SendQuestion(botClient, message, cancellationToken);
                return;
            }

            // Если в БД есть такой пользователь
            if (!user.FinishedForm())
            {
                MessageSender.SendFormStart(botClient, message, cancellationToken);

                Statuses.Add(message.Chat.Id, FormStatus.Name);
                user.Id = message.Chat.Id;
                Users.Add(message.Chat.Id, user);

                MessageSender.SendTextMessage(botClient, message, cancellationToken, "Введите ваше имя:");
            }
        }
    }

    // В случае получния ошибки
    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    // Опрос для пользователя
    private static async void HandleUserAnswersAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        switch (Statuses[message.Chat.Id])
        {
            case FormStatus.Name:
                TelegramBotModel.InputName(botClient, message, cancellationToken);
                break;

            case FormStatus.Surname:
                TelegramBotModel.InputSurname(botClient, message, cancellationToken);
                break;

            case FormStatus.DateOfBirth:
                TelegramBotModel.InputDateOfBirth(botClient, message, cancellationToken);
                break;

            case FormStatus.Goal:
                TelegramBotModel.InputGoal(botClient, message, cancellationToken);
                break;

            case FormStatus.Weight:
                if (int.TryParse(message.Text, out int weight))
                {
                    Users[message.Chat.Id].Weight = weight;
                    Statuses[message.Chat.Id] = FormStatus.Height;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "рост (в см)");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "вес");
                break;

            case FormStatus.Height:
                if (int.TryParse(message.Text, out int height))
                {
                    Users[message.Chat.Id].Height = height;
                    Statuses[message.Chat.Id] = FormStatus.Contraindications;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "противопоказания");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "рост");
                break;

            case FormStatus.Contraindications:
                Users[message.Chat.Id].Contraindications = message.Text;
                Statuses[message.Chat.Id] = FormStatus.HaveExp;
                await botClient.SendTextMessageAsync(message.Chat, "Есть ли у вас опыт в спорте?",
                    cancellationToken: cancellationToken);
                break;

            case FormStatus.HaveExp:
                Users[message.Chat.Id].HaveExp = message.Text;
                Statuses[message.Chat.Id] = FormStatus.Bust;
                MessageSender.SendInputMessage(botClient, message, cancellationToken, "обхват груди (в см)");
                break;

            case FormStatus.Bust:
                if (int.TryParse(message.Text, out int bust))
                {
                    Users[message.Chat.Id].Bust = bust;
                    Statuses[message.Chat.Id] = FormStatus.Waist;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "обхват талии (в см)");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "обхват груди");

                break;

            case FormStatus.Waist:
                if (int.TryParse(message.Text, out int waist))
                {
                    Users[message.Chat.Id].Waist = waist;
                    Statuses[message.Chat.Id] = FormStatus.Stomach;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "обхват живота (в см)");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "обхват талии");

                break;

            case FormStatus.Stomach:
                if (int.TryParse(message.Text, out int stomach))
                {
                    Users[message.Chat.Id].Stomach = stomach;
                    Statuses[message.Chat.Id] = FormStatus.Hips;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "обхват бёдер (в см)");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "обхват живота");

                break;

            case FormStatus.Hips:
                if (int.TryParse(message.Text, out int hips))
                {
                    Users[message.Chat.Id].Hips = hips;
                    Statuses[message.Chat.Id] = FormStatus.Legs;
                    MessageSender.SendInputMessage(botClient, message, cancellationToken, "обхват ноги (в см)");
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "обхват бёдер");

                break;

            case FormStatus.Legs:
                if (int.TryParse(message.Text, out int legs))
                {
                    Users[message.Chat.Id].Legs = legs;

                    Db.UpdateUser(Users[message.Chat.Id]);

                    // Очищаем из памяти, чтобы не засорять
                    Users.Remove(message.Chat.Id);
                    Statuses.Remove(message.Chat.Id);

                    MessageSender.SendFormFinish(botClient, message, cancellationToken);
                }
                else
                    MessageSender.SendFailureMessage(botClient, message, cancellationToken, "обхват ноги");

                break;
        }
    }

    // Обработчик ответов тренера
    private static async void HandleTrainerAnswersAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        Debug.Assert(message.Text != null, "message.Text != null");

        switch (TrainersActions[message.Chat.Id])
        {
            case ActionStatus.AddClientUsername:
                User user = new User(message.Text, message.Chat.Id);
                Db.AddUser(user);
                TrainersActions.Remove(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat,
                    "Клиент успешно добавлен в базу данных. Теперь ему необходимо заполнить анкету.",
                    cancellationToken: cancellationToken);

                TrainersActions.Remove(message.Chat.Id);
                break;

            case ActionStatus.DeleteClientByUsername:
                Db.DeleteClientByUsername(message.Text);
                TrainersActions.Remove(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat,
                    "Клиент успешно удалён из базы данных.",
                    cancellationToken: cancellationToken);

                TrainersActions.Remove(message.Chat.Id);
                break;

            case ActionStatus.AddTrainingDate:
                if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                        System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    Training training = new Training(dt, message.Chat.Id);
                    Trainings.Add(message.Chat.Id, training);
                    TrainersActions[message.Chat.Id] = ActionStatus.AddTrainingLocation;

                    await botClient.SendTextMessageAsync(message.Chat,
                        "Введите адрес места проведения тренировки:",
                        cancellationToken: cancellationToken);
                }
                else
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести дату, попробуйте снова:",
                        cancellationToken: cancellationToken);

                break;

            case ActionStatus.AddTrainingLocation:
                Trainings[message.Chat.Id].Location = message.Text;
                TrainersActions[message.Chat.Id] = ActionStatus.AddClientForTraining;

                await botClient.SendTextMessageAsync(message.Chat,
                    "Если вы хотите добавить окно для клиентов, то введите 'окно', иначе введите имя пользователя клиента в Telegram:",
                    cancellationToken: cancellationToken);
                break;

            case ActionStatus.AddClientForTraining:
                Trainings[message.Chat.Id].ClientUsername = message.Text;

                Db.AddTraining(Trainings[message.Chat.Id]);

                Trainings.Remove(message.Chat.Id);
                TrainersActions.Remove(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat,
                    message.Text == "окно" ? "Окно для тренировок успешно добавлено." : "Тренировка успешно добавлена.",
                    cancellationToken: cancellationToken);
                break;

            case ActionStatus.DeleteTrainingByTime:
                if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                        System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    Db.DeleteTrainingByDateTime(dateTime);
                    TrainersActions.Remove(message.Chat.Id);

                    await botClient.SendTextMessageAsync(message.Chat,
                        "Тренировка успешно удалена из базы данных.",
                        cancellationToken: cancellationToken);
                }
                else
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести дату, попробуйте снова:",
                        cancellationToken: cancellationToken);

                break;
        }
    }

    // Обработчик нажатых кнопок
    private static async void HandleCallbackQuery(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var queryMessage = update.CallbackQuery?.Message;

        if (queryMessage == null)
            return;

        switch (update.CallbackQuery?.Data)
        {
            case "t_timetable":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Выберите пункт меню:",
                    replyMarkup: MenuButtons.TrainerTimetableMenu(),
                    cancellationToken: cancellationToken);
                break;

            case "clients":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Выберите пункт меню:",
                    replyMarkup: MenuButtons.TrainerClientsMenu(),
                    cancellationToken: cancellationToken);
                break;

            case "add_training":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Чтобы добавить тренировку, введите время проведения в формате dd.MM.yyyy HH:mm:",
                    cancellationToken: cancellationToken);

                TrainersActions.Add(queryMessage.Chat.Id, ActionStatus.AddTrainingDate);
                break;

            case "cancel_training":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Чтобы удалить тренировку, введите время проведения в формате dd.MM.yyyy HH:mm:",
                    cancellationToken: cancellationToken);

                TrainersActions.Add(queryMessage.Chat.Id, ActionStatus.DeleteTrainingByTime);
                break;

            case "week_timetable":
                List<Training> trainings = Db.GetTrainingsByTrainerId(queryMessage.Chat.Id);
                DateTime now = DateTime.Now;

                List<Training> trainingsIn7Days =
                    trainings.Where(t => (t.Time >= now) && (t.Time <= now.AddDays(7)) && (t.ClientUsername != "окно"))
                        .ToList();

                var groupedTrainings = trainingsIn7Days.GroupBy(t => t.Time.DayOfWeek);

                StringBuilder timetable = new StringBuilder();

                foreach (var group in groupedTrainings)
                {
                    timetable.Append(group.Key).Append('\n');

                    foreach (var t in group)
                        timetable.Append(t).Append("\n\n");
                }

                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    timetable.ToString(),
                    cancellationToken: cancellationToken);

                break;

            case "add_client":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Чтобы добавить клиента, введите его имя пользователя в Telegram:",
                    cancellationToken: cancellationToken);

                TrainersActions.Add(queryMessage.Chat.Id, ActionStatus.AddClientUsername);
                break;

            case "delete_client":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Чтобы удалить клиента клиента, введите его имя пользователя в Telegram:",
                    cancellationToken: cancellationToken);

                TrainersActions.Add(queryMessage.Chat.Id, ActionStatus.DeleteClientByUsername);
                break;

            case "check_base":
                List<User> users = Db.GetAllClientsByTrainerId(queryMessage.Chat.Id);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < users.Count; i++)
                    sb.Append(i + 1).Append(") ").Append(users[i]).Append("\n\n");

                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    sb.ToString(),
                    cancellationToken: cancellationToken);

                break;

            case "cl_timetable":
                Console.WriteLine("HellO!");
                break;

            case "cl_form":
                break;

            case "i_am_trainer": // пришёл новый тренер
                Db.AddTrainer(new Trainer(queryMessage.Chat.Id, queryMessage.Chat.Username));

                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Инструкция для тренера (написать)",
                    replyMarkup: MenuButtons.TrainerMenu(),
                    cancellationToken: cancellationToken);
                break;

            case "i_am_client":
                await botClient.SendTextMessageAsync(queryMessage.Chat,
                    "Извините, тренер пока не добавил вас в список своих клиентов",
                    cancellationToken: cancellationToken);
                break;
        }
    }
}