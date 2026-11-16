using Pos.Core.Domain;
namespace Pos.Core.Application;

public sealed record StudentTotal(Student Student, double Total);

public interface IResultsExporter
{
    void Export(IReadOnlyList<StudentTotal> totals, string path);
}
