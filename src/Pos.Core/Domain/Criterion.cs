namespace Pos.Core.Domain;

public sealed class Criterion
{
    public CriterionId Id { get; }
    public string Name { get; }
    public int MaxScore { get; }
    public double Weight { get; }

    internal Criterion(CriterionId id, string name, int maxScore, double weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Название критерия не может быть пустым");
        if (maxScore <= 0)
            throw new DomainException("Максимальный балл критерия должен быть положительным");
        if (weight <= 0)
            throw new DomainException("Вес критерия должен быть положительным");

        Id = id;
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }
}
