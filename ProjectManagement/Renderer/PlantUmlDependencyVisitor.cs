using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

// PlantUML dependency diagram visitor with ID and Name labels
public class PlantUmlDependencyVisitor : IProjectVisitor, IResultProvider
{
    private readonly StringBuilder _sb = new();

    public void Start() => _sb.AppendLine("@startuml");

    public void Visit(TaskNode node, int level)
    {
        var nodeLabel = $"{Escape(node.Id)}: {Escape(node.Name)}";

        foreach (var dep in node.Dependencies)
        {
            var depLabel = $"{Escape(dep.Id)}: {Escape(dep.Name)}";
            _sb.AppendLine($"[{depLabel}] --> [{nodeLabel}]");
        }

        foreach (var rel in node.Relations)
        {
            foreach (var target in rel.Value)
            {
                var targetLabel = $"{Escape(target.Id)}: {Escape(target.Name)}";
                _sb.AppendLine($"[{nodeLabel}] -[#blue]-> [{targetLabel}] : {Escape(rel.Key)}");
            }
        }
    }

    public void End() => _sb.AppendLine("@enduml");

    public string GetResult() => _sb.ToString();

    private static string Escape(string text)
        => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
