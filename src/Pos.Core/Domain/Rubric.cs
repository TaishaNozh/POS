namespace Pos.Core.Domain;

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = new();

    public IReadOnlyList<Criterion> Criteria => _criteria;

    public double TotalWeight => _criteria.Sum(c => c.Weight);

    public Criterion AddCriterion(string name, int maxScore, double weight)
    {
        var criterion = new Criterion(CriterionId.New(), name, maxScore, weight);

        if (_criteria.Any(c => string.Equals(c.Name, criterion.Name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Критерий «{criterion.Name}» уже есть в рубрикаторе");

        _criteria.Add(criterion);
        return criterion;
    }

    public Criterion Get(CriterionId id) =>
        _criteria.FirstOrDefault(c => c.Id == id)
        ?? throw new DomainException("Оценку можно выставить только по критерию из рубрикатора");
}
