using Pos.Core.Application;
using Pos.Core.Domain;
namespace Pos.ConsoleApp;

internal sealed class ConsoleMenu
{
    private readonly GradingFacade _facade;
    private readonly DemoScenario _demo;

    public ConsoleMenu(GradingFacade facade, DemoScenario demo)
    {
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
        _demo = demo ?? throw new ArgumentNullException(nameof(demo));
    }

    public void Run()
    {
        Console.WriteLine("ПОС — Преподавательская Оценка Студентов");
        Console.WriteLine();

        while (true)
        {
            PrintMenu();
            var choice = ConsoleInput.ReadInt("Выберите пункт: ", 0, 9);
            Console.WriteLine();

            if (choice == 0)
            {
                Console.WriteLine("До свидания!");
                return;
            }

            try
            {
                Execute(choice);
            }
            catch (DomainException ex)
            {
                Console.WriteLine($"Нельзя: {ex.Message}");
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("1. Добавить студента");
        Console.WriteLine("2. Добавить критерий в рубрикатор");
        Console.WriteLine("3. Показать студентов и рубрикатор");
        Console.WriteLine("4. Выставить оценку");
        Console.WriteLine("5. Показать оценки студента");
        Console.WriteLine("6. Отменить оценку");
        Console.WriteLine("7. Итоги по всем студентам");
        Console.WriteLine("8. Экспорт итогов в CSV");
        Console.WriteLine("9. Демо: сквозной сценарий");
        Console.WriteLine("0. Выход");
    }

    private void Execute(int choice)
    {
        switch (choice)
        {
            case 1:
                AddStudent();
                break;
            case 2:
                AddCriterion();
                break;
            case 3:
                PrintStudentsAndRubric();
                break;
            case 4:
                Grade();
                break;
            case 5:
                PrintEvaluations();
                break;
            case 6:
                CancelEvaluation();
                break;
            case 7:
                PrintTotals(_facade.GetTotals());
                break;
            case 8:
                Export();
                break;
            case 9:
                _demo.Run();
                break;
        }
    }

    private void AddStudent()
    {
        var student = _facade.AddStudent(ConsoleInput.ReadText("Имя студента: "));
        Console.WriteLine($"Добавлен студент: {student.Name}");
    }

    private void AddCriterion()
    {
        var name = ConsoleInput.ReadText("Название критерия: ");
        var maxScore = ConsoleInput.ReadInt("Максимальный балл: ", 1, 1000);
        var weight = ConsoleInput.ReadPositiveDouble("Вес (вклад в итог, например 0.3): ");

        var criterion = _facade.AddCriterion(name, maxScore, weight);
        Console.WriteLine($"Добавлен критерий: {criterion.Name} (из {criterion.MaxScore}, вес {criterion.Weight})");
    }

    private void PrintStudentsAndRubric()
    {
        var students = _facade.GetStudents();
        Console.WriteLine(students.Count == 0 ? "Студентов пока нет" : "Студенты:");
        for (var i = 0; i < students.Count; i++)
            Console.WriteLine($"  {i + 1}. {students[i].Name}");

        Console.WriteLine();

        var criteria = _facade.GetCriteria();
        Console.WriteLine(criteria.Count == 0 ? "Рубрикатор пуст" : "Рубрикатор:");
        for (var i = 0; i < criteria.Count; i++)
            Console.WriteLine($"  {i + 1}. {criteria[i].Name} — из {criteria[i].MaxScore}, вес {criteria[i].Weight}");
    }

    private void Grade()
    {
        var student = ChooseStudent();
        var criterion = ChooseCriterion();

        var score = ConsoleInput.ReadInt($"Балл (максимум {criterion.MaxScore}): ", int.MinValue, int.MaxValue);

        var evaluation = _facade.Grade(student.Id, criterion.Id, score);
        Console.WriteLine($"Оценка выставлена: {student.Name}, «{criterion.Name}» — {evaluation.Score}/{criterion.MaxScore}");
    }

    private void PrintEvaluations()
    {
        var student = ChooseStudent();
        var evaluations = _facade.GetEvaluations(student.Id);

        if (evaluations.Count == 0)
        {
            Console.WriteLine($"У студента {student.Name} оценок пока нет");
            return;
        }

        Console.WriteLine($"Оценки студента {student.Name}:");
        foreach (var evaluation in evaluations)
        {
            var criterion = _facade.GetCriteria().First(c => c.Id == evaluation.CriterionId);
            Console.WriteLine($"  {criterion.Name}: {evaluation.Score}/{criterion.MaxScore}");
        }
        Console.WriteLine($"Итог: {_facade.GetTotal(student.Id):0.00} из 10");
    }

    private void CancelEvaluation()
    {
        var student = ChooseStudent();
        var criterion = ChooseCriterion();

        _facade.CancelEvaluation(student.Id, criterion.Id);
        Console.WriteLine("Оценка отменена");
    }

    private void Export()
    {
        var path = ConsoleInput.ReadText("Путь к CSV-файлу (например, results.csv): ").Trim('"', '\'');
        _facade.ExportTotals(path);
        Console.WriteLine($"Итоги сохранены: {Path.GetFullPath(path)}");
    }

    public static void PrintTotals(IReadOnlyList<StudentTotal> totals)
    {
        if (totals.Count == 0)
        {
            Console.WriteLine("Студентов пока нет");
            return;
        }

        Console.WriteLine("Итоги (из 10):");
        foreach (var total in totals)
            Console.WriteLine($"  {total.Student.Name,-20} {total.Total,6:0.00}");
    }

    private Student ChooseStudent()
    {
        var students = _facade.GetStudents();
        if (students.Count == 0)
            throw new DomainException("Сначала добавьте хотя бы одного студента");

        Console.WriteLine("Студенты:");
        for (var i = 0; i < students.Count; i++)
            Console.WriteLine($"  {i + 1}. {students[i].Name}");

        return students[ConsoleInput.ReadInt("Номер студента: ", 1, students.Count) - 1];
    }

    private Criterion ChooseCriterion()
    {
        var criteria = _facade.GetCriteria();
        if (criteria.Count == 0)
            throw new DomainException("Сначала добавьте хотя бы один критерий в рубрикатор");

        Console.WriteLine("Критерии:");
        for (var i = 0; i < criteria.Count; i++)
            Console.WriteLine($"  {i + 1}. {criteria[i].Name} (из {criteria[i].MaxScore})");

        return criteria[ConsoleInput.ReadInt("Номер критерия: ", 1, criteria.Count) - 1];
    }
}
