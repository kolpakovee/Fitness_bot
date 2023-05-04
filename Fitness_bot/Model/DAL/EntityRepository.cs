using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public class EntityRepository<T> : IRepository<T>
    where T : class, IDomainObject
{
    private readonly TelegramBotContext _context;

    public EntityRepository(TelegramBotContext context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>();
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void Update(T entity)
    {
        var updateEntity = _context.Set<T>().Find(entity.Identifier);
        if (updateEntity != null)
            _context.Entry(updateEntity).CurrentValues.SetValues(entity);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}