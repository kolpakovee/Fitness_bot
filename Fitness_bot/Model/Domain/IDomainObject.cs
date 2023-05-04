using System.ComponentModel.DataAnnotations;

namespace Fitness_bot.Model.Domain;

public interface IDomainObject
{
    [Key]
    public string Identifier { get; set; }
}