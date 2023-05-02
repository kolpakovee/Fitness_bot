using System.Data.SQLite;

namespace Fitness_bot.Model.DAL;

public class ClientRepository
{
    private readonly SQLiteConnection _connection;

    public ClientRepository(SQLiteConnection connection)
    {
        _connection = connection;
    }
    
    
}