using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL.Interfaces;

public interface ITrainerRepository
{
    public void AddTrainer(Trainer trainer);
    public Trainer? GetTrainerById(long trainerId);
}