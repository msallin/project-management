using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

// PlantUML WBS visitor
public class PlantUmlWbsVisitor : IProjectVisitor, IResultProvider
{
    private readonly StringBuilder _sb = new();
    public void Start()
    {
        _sb.AppendLine("@startwbs");
        _sb.AppendLine("* Project");
    }
    public void Visit(TaskNode node, int level)
    {
        if(node.Tags.IsMilestone(out _))
        {
            // If the node is a milestone, we don't want to emit it in WBS format
            return;
        }

        string id = Escape(node.Id);
        string name = Escape(node.Name);
        _sb.Append('*', level + 1);
        if (node.Tags.GetColor(out string? color))
        {
            _sb.Append($"[#{Escape(color!)}]");
        }
        _sb.Append(' ');
        _sb.AppendLine($"{id} {name}");
    }
    public void End() => _sb.AppendLine("@endwbs");
    public string GetResult() => _sb.ToString();
    private static string Escape(string text) => string.IsNullOrEmpty(text) ? text : text.Replace("*", "\\*");
}
