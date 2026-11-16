namespace Pos.Core.Domain;

public interface ITotalStrategy
{
    double Calculate(Rubric rubric, IReadOnlyList<Evaluation> evaluations);
}

public sealed class WeightedTotalStrategy : ITotalStrategy
{
    public double Calculate(Rubric rubric, IReadOnlyList<Evaluation> evaluations)
    {
        if (rubric.Criteria.Count == 0)
            return 0;

        var weightedSum = evaluations.Sum(e =>
        {
            var criterion = rubric.Get(e.CriterionId);
            return (double)e.Score.Value / criterion.MaxScore * criterion.Weight;
        });

        return TotalScoreService.Scale * weightedSum / rubric.TotalWeight;
    }
}

public sealed class GradedOnlyTotalStrategy : ITotalStrategy
{
    public double Calculate(Rubric rubric, IReadOnlyList<Evaluation> evaluations)
    {
        if (evaluations.Count == 0)
            return 0;

        double weightedSum = 0, gradedWeight = 0;

        foreach (var evaluation in evaluations)
        {
            var criterion = rubric.Get(evaluation.CriterionId);
            weightedSum += (double)evaluation.Score.Value / criterion.MaxScore * criterion.Weight;
            gradedWeight += criterion.Weight;
        }

        return TotalScoreService.Scale * weightedSum / gradedWeight;
    }
}
