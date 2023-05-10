using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public class UnitOfWork
{
    private TelegramBotContext _context = new();
    private EntityRepository<Client>? _clients;
    private EntityRepository<Trainer>? _trainers;
    private EntityRepository<Training>? _trainings;

    public IRepository<Client> Clients => _clients ??= new EntityRepository<Client>(_context);

    public IRepository<Trainer> Trainers => _trainers ??= new EntityRepository<Trainer>(_context);

    public IRepository<Training> Trainings => _trainings ??= new EntityRepository<Training>(_context);

    public bool SaveChanges()
    {
        try
        {
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Discard()
    {
        _context.Dispose();
        _context = new TelegramBotContext();
    }
}