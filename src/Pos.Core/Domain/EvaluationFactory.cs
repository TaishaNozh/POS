namespace Pos.Core.Domain;

public sealed class EvaluationFactory
{
    public Evaluation Create(Student student, Rubric rubric, CriterionId criterionId, int score)
    {
        var criterion = rubric.Get(criterionId);
        var value = new Score(score);

        if (value.Value > criterion.MaxScore)
            throw new DomainException(
                $"Балл {score} вне диапазона критерия «{criterion.Name}»: допустимо от 0 до {criterion.MaxScore}");

        return new Evaluation(student.Id, criterion.Id, value);
    }
}
