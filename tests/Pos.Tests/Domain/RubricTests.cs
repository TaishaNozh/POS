using Pos.Core.Domain;
namespace Pos.Tests.Domain;

public class RubricTests
{
    [Fact]
    public void AddCriterion_AddsToRubricAndIsFoundById()
    {
        var rubric = new Rubric();

        var criterion = rubric.AddCriterion("Код", 10, 0.5);

        Assert.Single(rubric.Criteria);
        Assert.Same(criterion, rubric.Get(criterion.Id));
        Assert.Equal(10, criterion.MaxScore);
        Assert.Equal(0.5, criterion.Weight);
    }

    [Theory]
    [InlineData("Код")]
    [InlineData("код")]
    [InlineData("  КОД  ")]
    public void AddCriterion_DuplicateName_Throws(string duplicate)
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Код", 10, 0.5);

        Assert.Throws<DomainException>(() => rubric.AddCriterion(duplicate, 5, 0.1));
        Assert.Single(rubric.Criteria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AddCriterion_NonPositiveMaxScore_Throws(int maxScore)
    {
        Assert.Throws<DomainException>(() => new Rubric().AddCriterion("Код", maxScore, 0.5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public void AddCriterion_NonPositiveWeight_Throws(double weight)
    {
        Assert.Throws<DomainException>(() => new Rubric().AddCriterion("Код", 10, weight));
    }

    [Fact]
    public void AddCriterion_EmptyName_Throws()
    {
        Assert.Throws<DomainException>(() => new Rubric().AddCriterion("   ", 10, 0.5));
    }

    [Fact]
    public void Get_UnknownId_Throws()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Код", 10, 0.5);

        Assert.Throws<DomainException>(() => rubric.Get(CriterionId.New()));
    }

    [Fact]
    public void TotalWeight_IsSumOfCriteriaWeights()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Код", 10, 0.5);
        rubric.AddCriterion("README", 5, 0.3);

        Assert.Equal(0.8, rubric.TotalWeight, precision: 10);
    }
}
