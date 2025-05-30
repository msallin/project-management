using ProjectManagement;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

Console.WriteLine("Project Management Application");

ProjectGraph project = Demo.LoadFromMarkdown();
//project = Demo.LoadFromCode();

Console.WriteLine(project.Export(new MindmapTagVisitor()));
Console.WriteLine(project.Export(new MarkdownTableVisitor()));
Console.WriteLine(project.Export(new PlantUmlWbsVisitor()));
Console.WriteLine(project.ExportTopologicalSort(new PlantUmlGanttVisitor()));
Console.WriteLine(project.Export(new DotVisitor()));
Console.WriteLine(project.Export(new MermaidVisitor()));
Console.WriteLine(project.Export(new PlantUmlDependencyVisitor()));
Console.WriteLine(project.Export(new CriticalPathVisitor(project)));