using System.Data.SQLite;

namespace Fitness_bot.Model.DAL;

public class TrainerRepository
{
    private readonly SQLiteConnection _connection;

    public TrainerRepository(SQLiteConnection connection)
    {
        _connection = connection;
    }

    public bool AddTrainer(Trainer trainer)
    {
        string query = $"INSERT INTO Trainers ('id', 'name') VALUES ({trainer.Id}, '{trainer.Username}')";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
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
}