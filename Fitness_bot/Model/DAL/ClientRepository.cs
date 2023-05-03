using System.Data.SQLite;
using Fitness_bot.Model.DAL.Interfaces;
using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL;

public class ClientRepository : IClientRepository
{
    private readonly TelegramBotContext _context;

    public ClientRepository(TelegramBotContext context)
    {
        _context = context;
    }

    public void AddClient(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
    }

    public Client? GetClientByUsername(string username)
    {
        var client = _context.Clients
            .FirstOrDefault(cl => cl.Username == username);

        return client;
    }

    public List<Client> GetAllClientsByTrainerId(long trainerId)
    {
        List<Client> users = _context.Clients
            .Where(cl => cl.TrainerId == trainerId)
            .ToList();

        return users;
    }

    public void UpdateClient(Client client)
    {
        Client? clientToUpdate = GetClientByUsername(client.Username);
        if (clientToUpdate != null)
        {
            var clientEntry = _context.Entry(clientToUpdate);
            clientEntry.CurrentValues.SetValues(client);
        }

        _context.SaveChanges();
    }

    public void DeleteClientByUsername(string username)
    {
        Client? client = GetClientByUsername(username);
        if (client != null)
            _context.Remove(client);
        
        _context.SaveChanges();
    }
}