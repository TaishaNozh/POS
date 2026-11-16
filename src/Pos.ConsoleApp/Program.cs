using Microsoft.Extensions.DependencyInjection;
using Pos.Core.Application;
using Pos.ConsoleApp;
using Pos.Core.Domain;
using Pos.Core.Infrastructure;

var services = new ServiceCollection();

services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();
services.AddSingleton<IResultsExporter, CsvResultsExporter>();

services.AddSingleton<ITotalStrategy, WeightedTotalStrategy>();
services.AddSingleton<TotalScoreService>();
services.AddSingleton<EvaluationFactory>();

services.AddSingleton<GradingFacade>();
services.AddSingleton<DemoScenario>();
services.AddSingleton<ConsoleMenu>();

using var provider = services.BuildServiceProvider();
provider.GetRequiredService<ConsoleMenu>().Run();
