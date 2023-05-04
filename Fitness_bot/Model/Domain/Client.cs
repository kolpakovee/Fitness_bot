using System.ComponentModel.DataAnnotations;

namespace Fitness_bot.Model.Domain;

public class Client : IDomainObject
{
    [Key] public string Identifier { get; set; }
    public long Id { get; set; }
    public long TrainerId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Goal { get; set; }
    public int Weight { get; set; }
    public int Height { get; set; }
    public string? Contraindications { get; set; }
    public string? HaveExp { get; set; }
    public int Bust { get; set; }
    public int Waist { get; set; }
    public int Stomach { get; set; }
    public int Hips { get; set; }
    public int Legs { get; set; }

    public Client(string identifier)
    {
        Identifier = identifier;
    }

    public Client(string identifier, long trainerId)
    {
        TrainerId = trainerId;
        Identifier = identifier;
    }

    public Client(long id, long trainerId, string identifier, string? name, string? surname, string? dateOfBirth,
        string? goal,
        int weight,
        int height, string? contraindications, string? haveExp, int bust, int waist, int stomach, int hips, int legs)
    {
        Id = id;
        TrainerId = trainerId;
        Identifier = identifier;
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        Goal = goal;
        Weight = weight;
        Height = height;
        Contraindications = contraindications;
        HaveExp = haveExp;
        Bust = bust;
        Waist = waist;
        Stomach = stomach;
        Hips = hips;
        Legs = legs;
    }

    public bool FinishedForm() => Legs != 0;

    public override string ToString()
    {
        if (!FinishedForm())
            return $"Клиент {Identifier} не прошёл анкету.";

        return
            $"{Name} {Surname}\n- Дата рождения: {DateOfBirth}\n- Цель: {Goal}\n- Вес (кг): {Weight}\n- Рост (см): {Height}\n- Противопоказания: {Contraindications}\n- Есть ли опыт? {HaveExp}\n- Обхват груди (см): {Bust}\n- Обхват талии (см): {Waist}\n- Обхват живота (см): {Stomach}\n- Обхват бёдер (см): {Hips}\n- Обхват ноги (см): {Legs}";
    }
}