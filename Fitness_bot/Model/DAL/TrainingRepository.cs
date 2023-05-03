using Fitness_bot.Model.DAL.Interfaces;
using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public class TrainingRepository : ITrainingRepository
{
    private readonly TelegramBotContext _context;

    public TrainingRepository(TelegramBotContext context)
    {
        _context = context;
    }

    public void AddTraining(Training training)
    {
        _context.Trainings.Add(training);
        _context.SaveChanges();
    }
    
    public List<Training> GetTrainingsByTrainerId(long trainerId)
    {
        List<Training> trainings = _context.Trainings
            .Where(tr => tr.TrainerId == trainerId)
            .ToList();

        return trainings;
    }
    
    public void DeleteTrainingByDateTime(DateTime dateTime)
    {
        Training? training = _context.Trainings.FirstOrDefault(tr => 
            tr.Time.Year == dateTime.Year && 
            tr.Time.Month == dateTime.Month &&
            tr.Time.Day == dateTime.Day && 
            tr.Time.Hour == dateTime.Hour &&
            tr.Time.Minute == dateTime.Minute);
        
        if (training != null)
        {
            _context.Trainings.Remove(training);
            _context.SaveChanges();
        }
    }
}