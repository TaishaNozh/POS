using Pos.Core.Domain;
namespace Pos.Core.Infrastructure;


public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new();

    public void Add(Student student) => _students.Add(student);

    public Student? Find(StudentId id) => _students.FirstOrDefault(s => s.Id == id);

    public IReadOnlyList<Student> GetAll() => _students;
}

public sealed class InMemoryRubricRepository : IRubricRepository
{
    private readonly Rubric _rubric = new();

    public Rubric Get() => _rubric;
}

public sealed class InMemoryEvaluationRepository : IEvaluationRepository
{
    private readonly List<Evaluation> _evaluations = new();

    public void Add(Evaluation evaluation) => _evaluations.Add(evaluation);

    public bool Remove(StudentId studentId, CriterionId criterionId) =>
        _evaluations.RemoveAll(e => e.StudentId == studentId && e.CriterionId == criterionId) > 0;

    public IReadOnlyList<Evaluation> GetByStudent(StudentId studentId) =>
        _evaluations.Where(e => e.StudentId == studentId).ToList();
}
