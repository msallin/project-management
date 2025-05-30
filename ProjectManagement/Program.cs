using ProjectManagement;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

Console.WriteLine("Project Management Application");

ProjectGraph project = Demo.LoadFromCode();
project = Demo.LoadFromMarkdown();

var directory = Directory.CreateDirectory("./output");
File.WriteAllText("./output/mindmap.puml", project.Export(new MindmapTagVisitor()));
File.WriteAllText("./output/wbs.puml", project.Export(new PlantUmlWbsVisitor()));
File.WriteAllText("./output/gantt.puml", project.ExportTopologicalSort(new PlantUmlGanttVisitor()));
File.WriteAllText("./output/dependencies.puml", project.Export(new PlantUmlDependencyVisitor()));
File.WriteAllText("./output/table.md", project.Export(new MarkdownTableVisitor()));
File.WriteAllText("./output/dag.gv", project.Export(new DotVisitor()));
File.WriteAllText("./output/mermaid", project.Export(new MermaidVisitor()));
File.WriteAllText("./output/criticalpath.md", project.Export(new CriticalPathVisitor(project)));
Console.WriteLine($"Exported to: {directory.FullName}");