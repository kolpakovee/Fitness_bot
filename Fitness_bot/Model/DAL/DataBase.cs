using System.Data;
using System.Data.SQLite;

namespace Fitness_bot.Model.DAL;

public class DataBase
{
    public SQLiteConnection Connection { get; }

    public DataBase()
    {
        Connection = new SQLiteConnection("Data Source=database.sqlite");

        if (!File.Exists("./database.sqlite"))
        {
            SQLiteConnection.CreateFile("database.sqlite");
            Console.WriteLine("DataBase file created!");
        }
        
        OpenConnection();
    }

    private void OpenConnection()
    {
        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }
    }

    private void CloseConnection()
    {
        if (Connection.State != ConnectionState.Closed)
        {
            Connection.Close();
        }
    }
    
    ~DataBase()
    {
        CloseConnection();
    }
}