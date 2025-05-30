using ProjectManagement.Core;
using ProjectManagement.Importer;
using ProjectManagement.Renderer;

namespace ProjectManagement.Demo;

public class DemoFromMarkdown
{
    public static void Execute()
    {
        Console.WriteLine("Project Management Application - Markdown Import Example");
        ProjectGraph project = new();
        var markdownImporter = new MarkdownImporter();
        markdownImporter.ImportFile("Demo/MoonMissionRoadmap.md", project);

        // Render all views
        Console.WriteLine("--- Markdown Table ---" + Environment.NewLine + project.Export(new MarkdownTableVisitor()));
        Console.WriteLine("--- PlantUML WBS ---" + Environment.NewLine + project.Export(new PlantUmlWbsVisitor()));
        Console.WriteLine("--- PlantUML Gantt ---" + Environment.NewLine + project.ExportTopologicalSort(new PlantUmlGanttVisitor()));
        Console.WriteLine("--- DOT ---" + Environment.NewLine + project.Export(new DotVisitor()));
        Console.WriteLine("--- Mermaid ---" + Environment.NewLine + project.Export(new MermaidVisitor()));
        Console.WriteLine("--- PlantUML Dep ---" + Environment.NewLine + project.Export(new PlantUmlDependencyVisitor()));
    }
}
