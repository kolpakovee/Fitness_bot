using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL.Interfaces;

public interface ITrainingRepository
{
    public void AddTraining(Training training);
    public List<Training> GetTrainingsByTrainerId(long trainerId);
    public void DeleteTrainingByDateTime(DateTime dateTime);
}