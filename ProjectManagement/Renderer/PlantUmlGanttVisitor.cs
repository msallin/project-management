using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

// PlantUML Gantt visitor
public class PlantUmlGanttVisitor : IProjectVisitor, IResultProvider
{
    private readonly StringBuilder _sb = new();

    public void Start() => _sb.AppendLine("@startgantt");

    public void Visit(TaskNode node, int level)
    {
        var id = Escape(node.Id);
        var name = Escape(node.Name);
        _sb.Append($"[{id} {name}] lasts {node.Duration.TotalDays} day");
        if (node.Tags.GetColor(out string? color))
        {
            _sb.Append($" and is colored in {color}");
        }
        _sb.AppendLine();

        foreach (var rel in node.Relations)
        {
            foreach (var tgt in rel.Value)
            {
                _sb.AppendLine($"[{id} {name}] {rel.Key} [{Escape(tgt.Id)} {Escape(tgt.Name)}]");
            }
        }

        foreach (var dep in node.Dependencies)
        {
            _sb.AppendLine($"[{Escape(dep.Id)} {Escape(dep.Name)}] -> [{id} {name}]");
        }
    }

    public void End() => _sb.AppendLine("@endgantt");

    public string GetResult() => _sb.ToString();

    private static string Escape(string text) => string.IsNullOrEmpty(text) ? text : text.Replace("|", "\\|").Replace("[", "\\[").Replace("]", "\\]");
}
