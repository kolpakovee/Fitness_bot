using System.ComponentModel.DataAnnotations;

namespace Fitness_bot.Model.Domain;

public class Trainer
{
    public string Username { get; set; }
    [Key]
    public long Id { get; set; }

    public Trainer(long id, string username)
    {
        Username = username;
        Id = id;
    }
}