using System.ComponentModel.DataAnnotations;

namespace Fitness_bot.Model.Domain;

public class Trainer : IDomainObject
{
    [Key]
    public string Identifier { get; set; }
    public long Id { get; set; }

    public Trainer(long id, string identifier)
    {
        Identifier = identifier;
        Id = id;
    }
}