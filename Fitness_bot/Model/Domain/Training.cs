using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Fitness_bot.Model.Domain;

public class Training : IDomainObject
{
    [Key]
    public string Identifier { get; set; }
    public long TrainerId { get; set; }
    public string? ClientUsername { get; set; }
    public string? Location { get; set; }

    public Training(string identifier)
    {
        Identifier = identifier;
    }

    public Training(string identifier, long trainerId)
    {
        TrainerId = trainerId;
        Identifier = identifier;
    }

    public Training(string identifier, long trainerId, string? clientUsername, string location)
    {
        TrainerId = trainerId;
        ClientUsername = clientUsername;
        Location = location;
        Identifier = identifier;
    }

    public Training(string identifier, long trainerId, string location)
    {
        TrainerId = trainerId;
        Location = location;
        Identifier = identifier;
    }

    public override string ToString() => $"⌚ {Identifier}\n📍 {Location}";
}