using System.ComponentModel.DataAnnotations;

namespace Fitness_bot.Model.Domain;

public class Training
{
    [Key]
    public long Id { get; set; }
    public DateTime Time { get; set; }
    public long TrainerId { get; set; }
    public string? ClientUsername { get; set; }
    public string? Location { get; set; }

    public Training(DateTime time, long trainerId)
    {
        Time = time;
        TrainerId = trainerId;
    }

    public Training(DateTime time, long trainerId, string? clientUsername, string location)
    {
        Time = time;
        TrainerId = trainerId;
        ClientUsername = clientUsername;
        Location = location;
    }

    public Training(DateTime time, long trainerId, string location)
    {
        Time = time;
        TrainerId = trainerId;
        Location = location;
    }

    public override string ToString() => $"⌚ {Time:HH:mm}\n📍 {Location}";
}