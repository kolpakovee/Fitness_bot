using System.Data;
using System.Data.SQLite;
using Fitness_bot.Model;

namespace Fitness_bot.Model.DAL;

public class DataBase
{
    private readonly SQLiteConnection _connection;

    public DataBase()
    {
        _connection = new SQLiteConnection("Data Source=database.sqlite");

        if (!File.Exists("./database.sqlite"))
        {
            SQLiteConnection.CreateFile("database.sqlite");
            Console.WriteLine("DataBase file created!");
        }
        
        OpenConnection();
    }

    public void OpenConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void CloseConnection()
    {
        if (_connection.State != ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    public void AddUser(User user)
    {
        string query =
            "INSERT INTO Users ('id', 'trainer_id', 'username', 'name', 'surname', 'dateOfBirth','goal', 'weight', 'height', 'contraindications', 'haveExp', 'bust', 'waist', 'stomach', 'hips', 'legs') " +
            $"VALUES ({user.Id}, {user.TrainerId}, '{user.Username}', '{user.Name}', '{user.Surname}', '{user.DateOfBirth}', '{user.Goal}', {user.Weight}, {user.Height}, '{user.Contraindications}', '{user.HaveExp}', {user.Bust}, {user.Waist}, {user.Stomach}, {user.Hips}, {user.Legs})";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    public void AddTrainer(Trainer trainer)
    {
        string query = $"INSERT INTO Trainers ('id', 'name') VALUES ({trainer.Id}, '{trainer.Username}')";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    public void AddTraining(Training training)
    {
        string query =
            $"INSERT INTO Trainings ('trainer_id', 'client_username', 'location', 'date_time') VALUES ({training.TrainerId}, '{training.ClientUsername}', '{training.Location}', '{training.Time}')";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    public Trainer? GetTrainerById(long trainerId)
    {
        string query = $"SELECT * FROM Trainers WHERE id={trainerId}";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        SQLiteDataReader dataReader = command.ExecuteReader();

        if (dataReader.HasRows)
        {
            while (dataReader.Read())
            {
                return new Trainer(dataReader.GetInt64(0), dataReader.GetString(1));
            }
        }

        return null;
    }

    public User? GetUserByUsername(string username)
    {
        string query = $"SELECT * FROM Users WHERE username='{username}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        SQLiteDataReader dataReader = command.ExecuteReader();

        if (dataReader.HasRows)
        {
            while (dataReader.Read())
            {
                return new User(dataReader.GetInt64(0), dataReader.GetInt64(1), dataReader.GetString(2),
                    dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5), dataReader.GetString(6),
                    dataReader.GetInt16(7), dataReader.GetInt16(8), dataReader.GetString(9), dataReader.GetString(10),
                    dataReader.GetInt16(11), dataReader.GetInt16(12), dataReader.GetInt16(13), dataReader.GetInt16(14),
                    dataReader.GetInt16(15));
            }
        }

        return null;
    }

    public List<User> GetAllClientsByTrainerId(long trainerId)
    {
        List<User> users = new List<User>();

        string query = $"SELECT * FROM Users WHERE trainer_id={trainerId}";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        SQLiteDataReader dataReader = command.ExecuteReader();

        if (dataReader.HasRows)
        {
            while (dataReader.Read())
                users.Add(new User(dataReader.GetInt64(0), dataReader.GetInt64(1), dataReader.GetString(2),
                    dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5), dataReader.GetString(6),
                    dataReader.GetInt16(7), dataReader.GetInt16(8), dataReader.GetString(9), dataReader.GetString(10),
                    dataReader.GetInt16(11), dataReader.GetInt16(12), dataReader.GetInt16(13), dataReader.GetInt16(14),
                    dataReader.GetInt16(15)));
        }

        return users;
    }

    public List<Training> GetTrainingsByTrainerId(long trainerId)
    {
        List<Training> trainings = new();

        string query = $"SELECT * FROM Trainings WHERE trainer_id={trainerId}";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        SQLiteDataReader dataReader = command.ExecuteReader();

        if (dataReader.HasRows)
        {
            while (dataReader.Read())
            {
                if (DateTime.TryParseExact(dataReader.GetString(3), "dd/MM/yyyy HH:mm:ss", null,
                        System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    trainings.Add(new Training(dateTime, dataReader.GetInt64(0), dataReader.GetString(1),
                        dataReader.GetString(2)));
                }
            }
        }

        return trainings;
    }

    public void UpdateUser(User user)
    {
        string query =
            $"UPDATE Users SET id={user.Id}, name='{user.Name}', surname='{user.Surname}', dateOfBirth ='{user.DateOfBirth}'," +
            $"goal='{user.Goal}', weight={user.Weight}, height={user.Height},  contraindications='{user.Contraindications}', haveExp='{user.HaveExp}'," +
            $"bust={user.Bust}, waist={user.Waist}, stomach={user.Stomach}, hips={user.Hips}, legs={user.Legs} WHERE username='{user.Username}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    public void DeleteClientByUsername(string username)
    {
        string query = $"DELETE FROM Users WHERE username='{username}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    public void DeleteTrainingByDateTime(DateTime dateTime)
    {
        string query = $"DELETE FROM Trainings WHERE date_time='{dateTime}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        command.ExecuteNonQuery();
    }

    ~DataBase()
    {
        CloseConnection();
    }
}