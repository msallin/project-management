using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

/// <summary>
///     PlantUML mindmap visitor grouping tasks by tag.
/// </summary>
public class MindmapTagVisitor : IProjectVisitor, IResultProvider
{
    private readonly StringBuilder _sb = new();
    private readonly HashSet<string> _emittedTags = [];
    private bool _untaggedEmitted = false;

    public void Start() => _sb.AppendLine("@startmindmap");

    public void Visit(TaskNode node, int level)
    {
        // Prepare the task label: "ID - Name"
        string label = $"{Escape(node.Id)} - {Escape(node.Name)}";

        // If the task has tags, emit under each
        IEnumerable<string> tags = node.Tags.GetNonControlTags();
        if (tags.Any())
        {
            foreach (string tag in node.Tags)
            {
                // Emit the tag header once
                if (_emittedTags.Add(tag))
                {
                    _sb.AppendLine($"* {Escape(tag)}");
                }

                AddTask(node, label);
            }
        }
        else
        {
            // Emit a single "Untagged" header, then this task
            if (!_untaggedEmitted)
            {
                _sb.AppendLine("* Untagged");
                _untaggedEmitted = true;
            }

            AddTask(node, label);
        }
    }

    private void AddTask(TaskNode node, string label)
    {
        _sb.Append($"**"); // Start the sub-task under this tag
        if (node.Tags.GetColor(out string? color)) // If the tag has a color, apply it
        {
            _sb.Append($"[#{Escape(color!)}]");
        }
        _sb.AppendLine($" {label}"); // Emit the task under that tag
    }

    public void End() => _sb.AppendLine("@endmindmap");

    public string GetResult() => _sb.ToString();

    private static string Escape(string text)
        => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
