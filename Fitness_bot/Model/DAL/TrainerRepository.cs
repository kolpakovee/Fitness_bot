using Fitness_bot.Model.DAL.Interfaces;
using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public class TrainerRepository : ITrainerRepository
{
    private readonly TelegramBotContext _context;

    public TrainerRepository(TelegramBotContext context)
    {
        _context = context;
    }

    public void AddTrainer(Trainer trainer)
    {
        _context.Trainers.Add(trainer);
        _context.SaveChanges();
    }
    
    public Trainer? GetTrainerById(long trainerId)
    {
        var trainer = _context.Trainers
            .FirstOrDefault(t => t.Id == trainerId);

        return trainer;
    }
}