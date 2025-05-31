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
        string nodeLabel = $"{Escape(node.Id)}: {Escape(node.Name)}";

        foreach (TaskNode dep in node.Dependencies)
        {
            string depLabel = $"{Escape(dep.Id)}: {Escape(dep.Name)}";
            _sb.AppendLine($"[{depLabel}] --> [{nodeLabel}]");
        }

        foreach (KeyValuePair<string, HashSet<TaskNode>> rel in node.Relations)
        {
            foreach (TaskNode target in rel.Value)
            {
                string targetLabel = $"{Escape(target.Id)}: {Escape(target.Name)}";
                _sb.AppendLine($"[{nodeLabel}] -[#blue]-> [{targetLabel}] : {Escape(rel.Key)}");
            }
        }
    }

    public void End() => _sb.AppendLine("@enduml");

    public string GetResult() => _sb.ToString();

    private static string Escape(string text)
        => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
