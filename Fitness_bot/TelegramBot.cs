using System.Diagnostics;
using System.Text;
using Fitness_bot.Enums;
using Fitness_bot.Model;
using Fitness_bot.View;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Fitness_bot.Model.User;

namespace Fitness_bot;

// TODO: написать комментарии со всеми входными параметрами
// TODO: сделать логгер

static class TelegramBot
{
    // Объект бота
    // TODO: спрятать в конфиг
    private static readonly ITelegramBotClient BotClient =
        new TelegramBotClient("5825304594:AAHQgVHuB4bfB01eetUGtXhGudpPMMLiRaI");

    // База данных
    private static readonly DataBase Db = new();

    // Списки, которые нужны для анкетирования новых пользователей
    private static readonly Dictionary<long, FormStatus> Statuses = new();
    private static readonly Dictionary<long, User> Users = new();
    private static readonly Dictionary<long, ActionStatus> TrainersActions = new ();
    private static readonly Dictionary<long, Training> Trainings = new();

    static void Main()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var receiverOptions = new ReceiverOptions();
        
        // Подключение к БД
        Db.OpenConnection();

        // Запуск бота
        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.ReadLine();

        // Закрытие подключение с БД
        Db.CloseConnection();
    }

    // Обработчик любого обновления
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
            HandleCallbackQuery(botClient, update, cancellationToken);

        // Если получили сообщение
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            // Проверили на null, чтобы дальше не было проблем
            if (message == null || message.Text == null) return;

            // Проверили, заполняет ли пользователь анкету
            if (Statuses.ContainsKey(message.Chat.Id))
            {
                HandleUserAnswersAsync(botClient, message, cancellationToken);
                return;
            }

            // Проверяем, тренер ли это
            Trainer? trainer = Db.GetTrainerById(message.Chat.Id);

            if (trainer != null) // пришёл зарегистрированный тренер
            {
                // если тренер 
                if (TrainersActions.ContainsKey(message.Chat.Id))
                {
                    HandleTrainerAnswersAsync(botClient, message, cancellationToken);
                    return;
                }
                
                await botClient.SendTextMessageAsync(message.Chat,
                    "Выберите пункт меню:",
                    replyMarkup: MenuButtons.TrainerMenu(),
                    cancellationToken: cancellationToken);
                return;
            }

            if (message.Chat.Username != null)
            {
                User? user = Db.GetUserByUsername(message.Chat.Username);

                // Если в БД нет такого пользователя
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat, 
                        "Здравствуйте! Вы тренер и хотите пользоваться телеграмм-ботом Leo?",
                        cancellationToken: cancellationToken,
                        replyMarkup: MenuButtons.YesOrNoButtons());
                    return;
                }

                // Если в БД есть такой пользователь
                if (!user.FinishedForm())
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Здравствуйте! Перед тем как начать пользоваться ботом необходимо заполнить небольшую анкету.",
                        cancellationToken: cancellationToken);

                    Statuses.Add(message.Chat.Id, FormStatus.Name);
                    user.Id = message.Chat.Id;
                    Users.Add(message.Chat.Id, user);

                    await botClient.SendTextMessageAsync(message.Chat, "Введите ваше имя:",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }

    // В случае получния ошибки
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    // Опрос для пользователя
    private static async void HandleUserAnswersAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        Debug.Assert(message.Text != null, "message.Text != null");

        switch (Statuses[message.Chat.Id])
        {
            case FormStatus.Name:
                Users[message.Chat.Id].Name = message.Text;
                Statuses[message.Chat.Id] = FormStatus.Surname;
                await botClient.SendTextMessageAsync(message.Chat, "Введите вашу фамилию:",
                    cancellationToken: cancellationToken);
                break;
            
            case FormStatus.Surname:
                Users[message.Chat.Id].Surname = message.Text;
                Statuses[message.Chat.Id] = FormStatus.DateOfBirth;
                await botClient.SendTextMessageAsync(message.Chat, "Введите вашу дату рождения:",
                    cancellationToken: cancellationToken);
                break;
            
            case FormStatus.DateOfBirth:
                Users[message.Chat.Id].DateOfBirth = message.Text;
                Statuses[message.Chat.Id] = FormStatus.Goal;
                await botClient.SendTextMessageAsync(message.Chat, "Введите цель тренировок:",
                    cancellationToken: cancellationToken);
                break;
            
            case FormStatus.Goal:
                Users[message.Chat.Id].Goal = message.Text;
                Statuses[message.Chat.Id] = FormStatus.Weight;
                await botClient.SendTextMessageAsync(message.Chat, "Введите ваш вес (в кг):",
                    cancellationToken: cancellationToken);
                break;
            
            case FormStatus.Weight:
                if (int.TryParse(message.Text, out int weight))
                {
                    Users[message.Chat.Id].Weight = weight;
                    Statuses[message.Chat.Id] = FormStatus.Height;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите ваш рост (в см):",
                        cancellationToken: cancellationToken);
                }
                else
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести вес, попробуйте снова:",
                        cancellationToken: cancellationToken);

                break;
            
            case FormStatus.Height:
                if (int.TryParse(message.Text, out int height))
                {
                    Users[message.Chat.Id].Height = height;
                    Statuses[message.Chat.Id] = FormStatus.Contraindications;
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Введите ваши противопоказания / операции:",
                        cancellationToken: cancellationToken);
                }

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
                await botClient.SendTextMessageAsync(message.Chat, "Введите обхват груди (в см):",
                    cancellationToken: cancellationToken);
                break;
            
            case FormStatus.Bust:
                if (int.TryParse(message.Text, out int bust))
                {
                    Users[message.Chat.Id].Bust = bust;
                    Statuses[message.Chat.Id] = FormStatus.Waist;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите обхват талии (в см):",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести обхват груди, попробуйте снова:",
                        cancellationToken: cancellationToken);
                }
                break;
            
            case FormStatus.Waist:
                if (int.TryParse(message.Text, out int waist))
                {
                    Users[message.Chat.Id].Waist = waist;
                    Statuses[message.Chat.Id] = FormStatus.Stomach;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите обхват живота (в см):",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести обхват талии, попробуйте снова:",
                        cancellationToken: cancellationToken);
                }
                break;
            
            case FormStatus.Stomach:
                if (int.TryParse(message.Text, out int stomach))
                {
                    Users[message.Chat.Id].Stomach = stomach;
                    Statuses[message.Chat.Id] = FormStatus.Hips;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите обхват бёдер (в см):",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести обхват живота, попробуйте снова:",
                        cancellationToken: cancellationToken);
                }
                break;
            
            case FormStatus.Hips:
                if (int.TryParse(message.Text, out int hips))
                {
                    Users[message.Chat.Id].Hips = hips;
                    Statuses[message.Chat.Id] = FormStatus.Legs;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите обхват ноги (в см):",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести обхват бёдер, попробуйте снова:",
                        cancellationToken: cancellationToken);
                }
                break;
            
            case FormStatus.Legs:
                if (int.TryParse(message.Text, out int legs))
                {
                    Users[message.Chat.Id].Legs = legs;

                    Db.UpdateUser(Users[message.Chat.Id]);

                    // Очищаем из памяти, чтобы не засорять
                    Users.Remove(message.Chat.Id);
                    Statuses.Remove(message.Chat.Id);

                    await botClient.SendTextMessageAsync(message.Chat,
                        "Вы успешно прошли анкету, теперь вам открыт основной функционал!",
                        replyMarkup: MenuButtons.ClientMenu(),
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Не удалось ввести обхват ноги, попробуйте снова:",
                        cancellationToken: cancellationToken);
                }
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
                    trainings.Where(t => (t.Time >= now) && (t.Time <= now.AddDays(7)) && (t.ClientUsername != "окно")).ToList();

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