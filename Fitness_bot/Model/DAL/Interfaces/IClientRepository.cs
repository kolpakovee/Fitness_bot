using Fitness_bot.Model.Domain;

namespace Fitness_bot.Model.DAL.Interfaces;

public interface IClientRepository
{
    public void AddClient(Client client);
    public Client? GetClientByUsername(string username);
    public List<Client> GetAllClientsByTrainerId(long trainerId);
    public void UpdateClient(Client client);
    public void DeleteClientByUsername(string username);
}