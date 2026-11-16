using Pos.Core.Domain;
namespace Pos.Tests.Domain;

public class TotalScoreServiceTests
{
    private readonly EvaluationFactory _factory = new();
    private readonly Student _student = new(StudentId.New(), "Иванова А.");
    private readonly Rubric _rubric = new();
    private readonly Criterion _code;
    private readonly Criterion _readme;
    private readonly Criterion _defense;

    public TotalScoreServiceTests()
    {
        _code = _rubric.AddCriterion("Код", 10, 0.5);
        _readme = _rubric.AddCriterion("README", 5, 0.3);
        _defense = _rubric.AddCriterion("Защита", 10, 0.2);
    }

    [Fact]
    public void Calculate_Weighted_AllCriteriaGraded()
    {
        var service = new TotalScoreService(new WeightedTotalStrategy());
        var evaluations = Grade((_code, 8), (_readme, 5), (_defense, 6));

        Assert.Equal(8.2, service.Calculate(_rubric, evaluations), precision: 10);
    }

    [Fact]
    public void Calculate_Weighted_UngradedCriterionCountsAsZero()
    {
        var service = new TotalScoreService(new WeightedTotalStrategy());
        var evaluations = Grade((_code, 8));

        Assert.Equal(4.0, service.Calculate(_rubric, evaluations), precision: 10);
    }

    [Fact]
    public void Calculate_GradedOnly_IgnoresUngradedCriteria()
    {
        var service = new TotalScoreService(new GradedOnlyTotalStrategy());
        var evaluations = Grade((_code, 8));

        Assert.Equal(8.0, service.Calculate(_rubric, evaluations), precision: 10);
    }

    [Fact]
    public void Calculate_NoEvaluations_ReturnsZero()
    {
        Assert.Equal(0, new TotalScoreService(new WeightedTotalStrategy()).Calculate(_rubric, Array.Empty<Evaluation>()));
        Assert.Equal(0, new TotalScoreService(new GradedOnlyTotalStrategy()).Calculate(_rubric, Array.Empty<Evaluation>()));
    }

    [Fact]
    public void Calculate_EmptyRubric_ReturnsZero()
    {
        var service = new TotalScoreService(new WeightedTotalStrategy());

        Assert.Equal(0, service.Calculate(new Rubric(), Array.Empty<Evaluation>()));
    }

    [Fact]
    public void Calculate_EvaluationForCriterionOutsideRubric_Throws()
    {
        var service = new TotalScoreService(new WeightedTotalStrategy());
        var otherRubric = new Rubric();
        var foreign = otherRubric.AddCriterion("Чужой", 10, 1);
        var evaluations = new[] { _factory.Create(_student, otherRubric, foreign.Id, 5) };

        Assert.Throws<DomainException>(() => service.Calculate(_rubric, evaluations));
    }

    private IReadOnlyList<Evaluation> Grade(params (Criterion Criterion, int Score)[] grades) =>
        grades.Select(g => _factory.Create(_student, _rubric, g.Criterion.Id, g.Score)).ToList();
}
