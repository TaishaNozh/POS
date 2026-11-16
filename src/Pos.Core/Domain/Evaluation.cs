namespace Pos.Core.Domain;

public sealed class Evaluation
{
    public StudentId StudentId { get; }
    public CriterionId CriterionId { get; }
    public Score Score { get; }

    internal Evaluation(StudentId studentId, CriterionId criterionId, Score score)
    {
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
    }
}
