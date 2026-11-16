using System.Globalization;
using System.Text;
using Pos.Core.Application;
namespace Pos.Core.Infrastructure;

public sealed class CsvResultsExporter : IResultsExporter
{
    public void Export(IReadOnlyList<StudentTotal> totals, string path)
    {
        var lines = new List<string> { "Студент;Итог" };
        lines.AddRange(totals.Select(t => $"{t.Student.Name};{t.Total.ToString("0.00", CultureInfo.InvariantCulture)}"));

        File.WriteAllLines(path, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }
}
