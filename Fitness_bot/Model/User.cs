namespace Fitness_bot.Model;

public class User
{
    public long Id { get; set; }
    public long TrainerId { get; set; }
    public string Username { get; set; }
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

    public User(string username, long trainerId)
    {
        TrainerId = trainerId;
        Username = username;
    }

    public User(long id, long trainerId, string username, string? name, string? surname, string? dateOfBirth,
        string? goal,
        int weight,
        int height, string? contraindications, string? haveExp, int bust, int waist, int stomach, int hips, int legs)
    {
        Id = id;
        TrainerId = trainerId;
        Username = username;
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

    public bool FinishedForm()
    {
        return Name != "" && Surname != "" && DateOfBirth != "" && Goal != "" && Contraindications != "" &&
               HaveExp != "";
    }

    public override string ToString()
    {
        if (!FinishedForm())
            return $"Клиент {Username} не прошёл анкету.";

        return
            $"{Name} {Surname}\n- Дата рождения: {DateOfBirth}\n- Цель: {Goal}\n- Вес (кг): {Weight}\n- Рост (см): {Weight}\n- Противопоказания: {Contraindications}\n- Есть ли опыт? {HaveExp}\n- Обхват груди (см): {Bust}\n- Обхват талии (см): {Waist}\n- Обхват живота (см): {Stomach}\n- Обхват бёдер (см): {Hips}\n- Обхват ноги (см): {Legs}";
    }
}