using Microsoft.Extensions.DependencyInjection;
using Pos.Core.Application;
using Pos.Core.Domain;
using Pos.Core.Infrastructure;
using Pos.Tests.Application;
namespace Pos.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void Container_ResolvesFacade_WithSharedRepositories()
    {
        using var provider = BuildProvider<WeightedTotalStrategy>();

        var facade = provider.GetRequiredService<GradingFacade>();
        var students = provider.GetRequiredService<IStudentRepository>();

        var student = facade.AddStudent("Иванова А.");

        Assert.Same(student, students.Find(student.Id));
    }

    [Fact]
    public void Container_WithGradedOnlyStrategy_FacadeUsesIt()
    {
        using var provider = BuildProvider<GradedOnlyTotalStrategy>();
        var facade = provider.GetRequiredService<GradingFacade>();

        var student = facade.AddStudent("Иванова А.");
        var code = facade.AddCriterion("Код", 10, 0.5);
        facade.AddCriterion("README", 5, 0.5);
        facade.Grade(student.Id, code.Id, 8);

        Assert.Equal(8.0, facade.GetTotal(student.Id), precision: 10);
    }

    private static ServiceProvider BuildProvider<TStrategy>() where TStrategy : class, ITotalStrategy
    {
        var services = new ServiceCollection();

        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
        services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();
        services.AddSingleton<IResultsExporter, FakeResultsExporter>();
        services.AddSingleton<ITotalStrategy, TStrategy>();
        services.AddSingleton<TotalScoreService>();
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<GradingFacade>();

        return services.BuildServiceProvider();
    }
}
