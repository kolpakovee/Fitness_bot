using Fitness_bot.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace Fitness_bot.Model.DAL;

public class TelegramBotContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Training> Trainings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=../../../fitness_bot.sqlite");
    }
}