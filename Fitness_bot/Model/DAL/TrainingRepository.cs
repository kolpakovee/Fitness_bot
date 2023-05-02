using System.Data.SQLite;

namespace Fitness_bot.Model.DAL;

public class TrainingRepository
{
    private readonly SQLiteConnection _connection;

    public TrainingRepository(SQLiteConnection connection)
    {
        _connection = connection;
    }
    
    public bool AddTraining(Training training)
    {
        string query =
            $"INSERT INTO Trainings ('trainer_id', 'client_username', 'location', 'date_time') VALUES ({training.TrainerId}, '{training.ClientUsername}', '{training.Location}', '{training.Time}')";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
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
    
    public bool DeleteTrainingByDateTime(DateTime dateTime)
    {
        string query = $"DELETE FROM Trainings WHERE date_time='{dateTime}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
    }
}