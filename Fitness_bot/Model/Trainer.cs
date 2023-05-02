namespace Fitness_bot.Model;

public class Trainer
{
    public string Username { get; set; }
    public long Id { get; set; }

    public Trainer(long id, string username)
    {
        Username = username;
        Id = id;
    }
}