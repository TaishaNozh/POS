using Pos.Core.Application;
using Pos.Core.Domain;
namespace Pos.ConsoleApp;

internal sealed class DemoScenario
{
    private const string ExportPath = "demo-results.csv";

    private readonly GradingFacade _facade;

    public DemoScenario(GradingFacade facade)
    {
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
    }

    public void Run()
    {
        if (_facade.GetStudents().Count > 0 || _facade.GetCriteria().Count > 0)
            throw new DomainException("Демо-сценарий запускается на пустых данных — перезапустите программу");

        Console.WriteLine("=== 1. Заводим студентов и критерии ===");
        var ivanova = _facade.AddStudent("Иванова А.");
        var petrov = _facade.AddStudent("Петров Б.");
        var sidorova = _facade.AddStudent("Сидорова В.");
        Console.WriteLine($"  студенты: {ivanova.Name}, {petrov.Name}, {sidorova.Name}");

        var code = _facade.AddCriterion("Код", 10, 0.5);
        var readme = _facade.AddCriterion("README", 5, 0.3);
        var defense = _facade.AddCriterion("Защита", 10, 0.2);
        foreach (var c in new[] { code, readme, defense })
            Console.WriteLine($"  критерий: {c.Name} — из {c.MaxScore}, вес {c.Weight}");

        Console.WriteLine();
        Console.WriteLine("=== 2. Выставляем оценки ===");
        Grade(ivanova, code, 8);
        Grade(ivanova, readme, 5);
        Grade(ivanova, defense, 6);
        Grade(petrov, code, 10);
        Grade(petrov, readme, 3);
        Grade(petrov, defense, 9);
        Grade(sidorova, code, 7);
        Grade(sidorova, readme, 4);
        Console.WriteLine("  (у Сидоровой «Защита» не оценена — в итоге учтётся как 0)");

        Console.WriteLine();
        Console.WriteLine("=== 3. Исправляем оценку: Иванова, «Код» 8 → 9 ===");
        Grade(ivanova, code, 9);

        Console.WriteLine();
        Console.WriteLine("=== 4. Пробуем нарушить правила домена ===");
        TryGrade(ivanova, code.Id, 12, "12 баллов из 10");
        TryGrade(ivanova, code.Id, -1, "отрицательный балл");
        TryGrade(ivanova, CriterionId.New(), 5, "критерий не из рубрикатора");

        Console.WriteLine();
        Console.WriteLine("=== 5. Итог (взвешенный, по 10-балльной шкале) ===");
        ConsoleMenu.PrintTotals(_facade.GetTotals());

        Console.WriteLine();
        Console.WriteLine("=== 6. Экспорт итога ===");
        _facade.ExportTotals(ExportPath);
        Console.WriteLine($"Итоги сохранены: {Path.GetFullPath(ExportPath)}");
    }

    private void Grade(Student student, Criterion criterion, int score)
    {
        _facade.Grade(student.Id, criterion.Id, score);
        Console.WriteLine($"  {student.Name}, «{criterion.Name}»: {score}/{criterion.MaxScore}");
    }

    private void TryGrade(Student student, CriterionId criterionId, int score, string what)
    {
        try
        {
            _facade.Grade(student.Id, criterionId, score);
            Console.WriteLine($"  {what}: ПРОШЛО (так быть не должно!)");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"  {what}: отклонено — {ex.Message}");
        }
    }
}
