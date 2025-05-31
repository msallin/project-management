using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer
{
    /// <summary>
    ///     PlantUML Gantt visitor that:
    ///     - prints normal tasks as “[id name] lasts X day”
    ///     - prints tasks tagged “Milestone” as “[id name] is milestone”
    ///     - optionally appends “and is colored in <color>” if the node has a color‐tag
    /// </summary>
    public class PlantUmlGanttVisitor : IProjectVisitor, IResultProvider
    {
        private readonly StringBuilder _sb = new();

        public void Start()
            => _sb.AppendLine("@startgantt");

        public void Visit(TaskNode node, int level)
        {
            string id = Escape(node.Id);
            string name = Escape(node.Name);

            // If the task has a tag “Milestone”, emit as a milestone.
            // Otherwise, emit as a duration‐based task.
            bool isMilestone = node.Tags.IsMilestone(out string? milestoneName);

            if (!isMilestone)
            {
                _sb.Append($"[{id} {name}] lasts {node.Duration.TotalDays} day");

                // Preserve existing color logic: if Tags.GetColor(...) returns a color, append “… and is colored in <color>”
                if (node.Tags.GetColor(out string? color))
                {
                    _sb.Append($" and is colored in {color}");
                }

                _sb.AppendLine();

                // Emit any “relations” (e.g., “--” or “-->” lines) exactly as before:
                foreach (KeyValuePair<string, HashSet<TaskNode>> rel in node.Relations)
                {
                    foreach (TaskNode tgt in rel.Value)
                    {
                        _sb.AppendLine(
                            $"[{id} {name}] {rel.Key} [{Escape(tgt.Id)} {Escape(tgt.Name)}]"
                        );
                    }

                }
            }

            // Emit dependencies as “[dep] -> [this]” exactly as before:
            foreach (TaskNode dep in node.Dependencies)
            {
                if (isMilestone)
                {
                    _sb.AppendLine(
                        $"[{id} {name}] happens at [{Escape(dep.Id)} {Escape(dep.Name)}]'s end"
                    );
                }
                else
                {
                    _sb.AppendLine(
                        $"[{Escape(dep.Id)} {Escape(dep.Name)}] -> [{id} {name}]"
                    );
                }
            }
        }

        public void End()
            => _sb.AppendLine("@endgantt");

        public string GetResult()
            => _sb.ToString();

        /// <summary>
        /// Simple escape to avoid PlantUML meta‐characters.
        /// </summary>
        private static string Escape(string text)
            => string.IsNullOrEmpty(text)
                ? text
                : text.Replace("|", "\\|")
                      .Replace("[", "\\[")
                      .Replace("]", "\\]");
    }
}
