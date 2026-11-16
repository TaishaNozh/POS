using Pos.Core.Domain;
namespace Pos.Tests.Domain;

public class EvaluationFactoryTests
{
    private readonly EvaluationFactory _factory = new();
    private readonly Student _student = new(StudentId.New(), "Иванова А.");
    private readonly Rubric _rubric = new();
    private readonly Criterion _code;

    public EvaluationFactoryTests()
    {
        _code = _rubric.AddCriterion("Код", 10, 0.5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ScoreWithinRange_CreatesEvaluation(int score)
    {
        var evaluation = _factory.Create(_student, _rubric, _code.Id, score);

        Assert.Equal(_student.Id, evaluation.StudentId);
        Assert.Equal(_code.Id, evaluation.CriterionId);
        Assert.Equal(new Score(score), evaluation.Score);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(100)]
    public void Create_ScoreAboveMax_Throws(int score)
    {
        Assert.Throws<DomainException>(() => _factory.Create(_student, _rubric, _code.Id, score));
    }

    [Fact]
    public void Create_NegativeScore_Throws()
    {
        Assert.Throws<DomainException>(() => _factory.Create(_student, _rubric, _code.Id, -1));
    }

    [Fact]
    public void Create_CriterionNotInRubric_Throws()
    {
        var foreign = new Rubric().AddCriterion("Чужой критерий", 10, 1);

        Assert.Throws<DomainException>(() => _factory.Create(_student, _rubric, foreign.Id, 5));
    }
}
