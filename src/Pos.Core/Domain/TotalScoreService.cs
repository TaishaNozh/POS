namespace Pos.Core.Domain;

public sealed class TotalScoreService
{
    public const int Scale = 10;

    private readonly ITotalStrategy _strategy;

    public TotalScoreService(ITotalStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public double Calculate(Rubric rubric, IReadOnlyList<Evaluation> evaluations)
    {
        foreach (var evaluation in evaluations)
            rubric.Get(evaluation.CriterionId);

        return _strategy.Calculate(rubric, evaluations);
    }
}
