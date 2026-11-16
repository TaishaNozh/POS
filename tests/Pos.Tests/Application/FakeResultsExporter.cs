using Pos.Core.Application;
namespace Pos.Tests.Application;

internal sealed class FakeResultsExporter : IResultsExporter
{
    public IReadOnlyList<StudentTotal>? LastTotals { get; private set; }
    public string? LastPath { get; private set; }

    public void Export(IReadOnlyList<StudentTotal> totals, string path)
    {
        LastTotals = totals;
        LastPath = path;
    }
}
