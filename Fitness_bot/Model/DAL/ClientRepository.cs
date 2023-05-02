using System.Data.SQLite;

namespace Fitness_bot.Model.DAL;

public class ClientRepository
{
    private readonly SQLiteConnection _connection;

    public ClientRepository(SQLiteConnection connection)
    {
        _connection = connection;
    }
    
    public bool AddClient(User user)
    {
        string query =
            "INSERT INTO Users ('id', 'trainer_id', 'username', 'name', 'surname', 'dateOfBirth','goal', 'weight', 'height', 'contraindications', 'haveExp', 'bust', 'waist', 'stomach', 'hips', 'legs') " +
            $"VALUES ({user.Id}, {user.TrainerId}, '{user.Username}', '{user.Name}', '{user.Surname}', '{user.DateOfBirth}', '{user.Goal}', {user.Weight}, {user.Height}, '{user.Contraindications}', '{user.HaveExp}', {user.Bust}, {user.Waist}, {user.Stomach}, {user.Hips}, {user.Legs})";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
    }
    
    public User? GetClientByUsername(string username)
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
    
    public bool UpdateClient(User user)
    {
        string query =
            $"UPDATE Users SET id={user.Id}, name='{user.Name}', surname='{user.Surname}', dateOfBirth ='{user.DateOfBirth}'," +
            $"goal='{user.Goal}', weight={user.Weight}, height={user.Height},  contraindications='{user.Contraindications}', haveExp='{user.HaveExp}'," +
            $"bust={user.Bust}, waist={user.Waist}, stomach={user.Stomach}, hips={user.Hips}, legs={user.Legs} WHERE username='{user.Username}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
    }
    
    public bool DeleteClientByUsername(string username)
    {
        string query = $"DELETE FROM Users WHERE username='{username}'";

        SQLiteCommand command = new SQLiteCommand(query, _connection);

        int count = command.ExecuteNonQuery();

        return count != 0;
    }
}