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
        var id = Escape(node.Id);
        var name = Escape(node.Name);
        _sb.Append('*', level + 1);
        _sb.Append(' ');
        _sb.AppendLine($"{id} {name}");
    }
    public void End() => _sb.AppendLine("@endwbs");
    public string GetResult() => _sb.ToString();
    private static string Escape(string text) => string.IsNullOrEmpty(text) ? text : text.Replace("*", "\\*");
}
