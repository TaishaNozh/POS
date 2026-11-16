namespace Pos.Core.Domain;

public interface IStudentRepository
{
    void Add(Student student);

    Student? Find(StudentId id);

    IReadOnlyList<Student> GetAll();
}

public interface IRubricRepository
{
    Rubric Get();
}

public interface IEvaluationRepository
{
    void Add(Evaluation evaluation);

    bool Remove(StudentId studentId, CriterionId criterionId);

    IReadOnlyList<Evaluation> GetByStudent(StudentId studentId);
}
