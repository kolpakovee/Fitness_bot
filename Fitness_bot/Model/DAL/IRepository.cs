using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public interface IRepository<T> where T : class, IDomainObject
{
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Delete(T entity);
    void Update(T entity);
    void Save();
}