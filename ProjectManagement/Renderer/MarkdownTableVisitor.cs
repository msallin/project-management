using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

// Markdown table visitor
public class MarkdownTableVisitor : IProjectVisitor, IResultProvider
{
    private readonly StringBuilder _sb = new();
    public void Start()
    {
        _sb.AppendLine("| Level | ID | Name | Description | Tags | Relations |");
        _sb.AppendLine("|---|---|---|---|---|---|");
    }
    public void Visit(TaskNode node, int level)
    {
        string desc = Escape(node.Description);
        string tags = string.Join(",", node.Tags);
        string rels = string.Join(", ", node.Relations.SelectMany(r => r.Value.Select(t => $"{r.Key}:{t.Id}")));
        _sb.AppendLine($"| {level} | {Escape(node.Id)} | {Escape(node.Name)} | {desc} | {tags} | {rels} |");
    }
    public void End() { }
    public string GetResult() => _sb.ToString();
    private static string Escape(string text) => string.IsNullOrEmpty(text) ? text : text.Replace("|", "\\|");
}
