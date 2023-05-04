using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Fitness_bot.Model.Domain;

public class Training : IDomainObject
{
    [Key]
    public string Identifier { get; set; }
    public DateTime Time { get; set; }
    public long TrainerId { get; set; }
    public string? ClientUsername { get; set; }
    public string? Location { get; set; }

    public Training(DateTime time)
    {
        Identifier = time.ToString("dd.MM.yyyy HH:mm");
    }

    public Training(DateTime time, long trainerId)
    {
        Time = time;
        TrainerId = trainerId;
        Identifier = time.ToString("dd.MM.yyyy HH:mm");
    }

    public Training(DateTime time, long trainerId, string? clientUsername, string location)
    {
        Time = time;
        TrainerId = trainerId;
        ClientUsername = clientUsername;
        Location = location;
        Identifier = time.ToString("dd.MM.yyyy HH:mm");
    }

    public Training(DateTime time, long trainerId, string location)
    {
        Time = time;
        TrainerId = trainerId;
        Location = location;
        Identifier = time.ToString("dd.MM.yyyy HH:mm");
    }

    public override string ToString() => $"⌚ {Time:HH:mm}\n📍 {Location}";
}