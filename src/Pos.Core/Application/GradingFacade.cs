using Pos.Core.Domain;
namespace Pos.Core.Application;

public sealed class GradingFacade
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;
    private readonly IEvaluationRepository _evaluations;
    private readonly EvaluationFactory _factory;
    private readonly TotalScoreService _totals;
    private readonly IResultsExporter _exporter;

    public GradingFacade(
        IStudentRepository students,
        IRubricRepository rubrics,
        IEvaluationRepository evaluations,
        EvaluationFactory factory,
        TotalScoreService totals,
        IResultsExporter exporter)
    {
        _students = students ?? throw new ArgumentNullException(nameof(students));
        _rubrics = rubrics ?? throw new ArgumentNullException(nameof(rubrics));
        _evaluations = evaluations ?? throw new ArgumentNullException(nameof(evaluations));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _totals = totals ?? throw new ArgumentNullException(nameof(totals));
        _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
    }

    public Student AddStudent(string name)
    {
        var student = new Student(StudentId.New(), name);
        _students.Add(student);
        return student;
    }

    public IReadOnlyList<Student> GetStudents() => _students.GetAll();

    public Criterion AddCriterion(string name, int maxScore, double weight) =>
        _rubrics.Get().AddCriterion(name, maxScore, weight);

    public IReadOnlyList<Criterion> GetCriteria() => _rubrics.Get().Criteria;

    public Evaluation Grade(StudentId studentId, CriterionId criterionId, int score)
    {
        var evaluation = _factory.Create(GetStudent(studentId), _rubrics.Get(), criterionId, score);

        _evaluations.Remove(studentId, criterionId);
        _evaluations.Add(evaluation);
        return evaluation;
    }

    public IReadOnlyList<Evaluation> GetEvaluations(StudentId studentId) =>
        _evaluations.GetByStudent(GetStudent(studentId).Id);

    public void CancelEvaluation(StudentId studentId, CriterionId criterionId)
    {
        if (!_evaluations.Remove(studentId, criterionId))
            throw new DomainException("По этому критерию оценки нет");
    }

    public double GetTotal(StudentId studentId) =>
        _totals.Calculate(_rubrics.Get(), GetEvaluations(studentId));

    public IReadOnlyList<StudentTotal> GetTotals() =>
        _students.GetAll()
            .Select(student => new StudentTotal(student, GetTotal(student.Id)))
            .ToList();

    public void ExportTotals(string path) => _exporter.Export(GetTotals(), path);

    private Student GetStudent(StudentId id) =>
        _students.Find(id) ?? throw new DomainException("Студент не найден");
}
