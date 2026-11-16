using Pos.Core.Application;
using Pos.Core.Domain;
using Pos.Core.Infrastructure;

namespace Pos.Tests.Application;

public class GradingFacadeTests
{
    private readonly FakeResultsExporter _exporter = new();
    private readonly GradingFacade _facade;

    public GradingFacadeTests()
    {
        _facade = new GradingFacade(
            new InMemoryStudentRepository(),
            new InMemoryRubricRepository(),
            new InMemoryEvaluationRepository(),
            new EvaluationFactory(),
            new TotalScoreService(new WeightedTotalStrategy()),
            _exporter);
    }

    [Fact]
    public void Grade_FirstTime_CreatesEvaluation()
    {
        var student = _facade.AddStudent("Иванова А.");
        var code = _facade.AddCriterion("Код", 10, 1);

        var evaluation = _facade.Grade(student.Id, code.Id, 8);

        Assert.Equal(new Score(8), evaluation.Score);
        Assert.Single(_facade.GetEvaluations(student.Id));
    }

    [Fact]
    public void Grade_SameCriterionAgain_ReplacesPreviousEvaluation()
    {
        var student = _facade.AddStudent("Иванова А.");
        var code = _facade.AddCriterion("Код", 10, 1);
        _facade.Grade(student.Id, code.Id, 8);

        _facade.Grade(student.Id, code.Id, 9);

        var evaluation = Assert.Single(_facade.GetEvaluations(student.Id));
        Assert.Equal(new Score(9), evaluation.Score);
    }

    [Fact]
    public void Grade_InvalidScore_DoesNotTouchExistingEvaluation()
    {
        var student = _facade.AddStudent("Иванова А.");
        var code = _facade.AddCriterion("Код", 10, 1);
        _facade.Grade(student.Id, code.Id, 8);

        Assert.Throws<DomainException>(() => _facade.Grade(student.Id, code.Id, 12));

        var evaluation = Assert.Single(_facade.GetEvaluations(student.Id));
        Assert.Equal(new Score(8), evaluation.Score);
    }

    [Fact]
    public void Grade_UnknownStudent_Throws()
    {
        var code = _facade.AddCriterion("Код", 10, 1);

        Assert.Throws<DomainException>(() => _facade.Grade(StudentId.New(), code.Id, 5));
    }

    [Fact]
    public void CancelEvaluation_RemovesIt()
    {
        var student = _facade.AddStudent("Иванова А.");
        var code = _facade.AddCriterion("Код", 10, 1);
        _facade.Grade(student.Id, code.Id, 8);

        _facade.CancelEvaluation(student.Id, code.Id);

        Assert.Empty(_facade.GetEvaluations(student.Id));
        Assert.Throws<DomainException>(() => _facade.CancelEvaluation(student.Id, code.Id));
    }

    [Fact]
    public void GetTotals_ReturnsTotalPerStudent()
    {
        var ivanova = _facade.AddStudent("Иванова А.");
        var petrov = _facade.AddStudent("Петров Б.");
        var code = _facade.AddCriterion("Код", 10, 0.5);
        var readme = _facade.AddCriterion("README", 5, 0.5);
        _facade.Grade(ivanova.Id, code.Id, 10);
        _facade.Grade(ivanova.Id, readme.Id, 5);
        _facade.Grade(petrov.Id, code.Id, 5);

        var totals = _facade.GetTotals();

        Assert.Equal(2, totals.Count);
        Assert.Equal(10.0, totals.Single(t => t.Student.Id == ivanova.Id).Total, precision: 10);
        Assert.Equal(2.5, totals.Single(t => t.Student.Id == petrov.Id).Total, precision: 10);
    }

    [Fact]
    public void ExportTotals_PassesTotalsToExporterPort()
    {
        var student = _facade.AddStudent("Иванова А.");
        var code = _facade.AddCriterion("Код", 10, 1);
        _facade.Grade(student.Id, code.Id, 7);

        _facade.ExportTotals("out.csv");

        Assert.Equal("out.csv", _exporter.LastPath);
        Assert.NotNull(_exporter.LastTotals);
        Assert.Equal(7.0, _exporter.LastTotals!.Single().Total, precision: 10);
    }
}
