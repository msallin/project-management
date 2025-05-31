using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer
{
    /// <summary>
    ///     DOT (GraphViz) visitor generating a graph with dependencies and parent-child relationships.
    /// </summary>
    public class DotVisitor : IProjectVisitor, IResultProvider
    {
        private readonly StringBuilder _sb = new();

        public void Start()
        {
            _sb.AppendLine("digraph Project {");
            _sb.AppendLine("  node [shape=box style=filled, fillcolor=\"white\", color=\"black\"]");
        }

        public void Visit(TaskNode node, int level)
        {
            string nodeLabel = GetLabel(node.Id, node.Name);
            string? fill = node.Tags.GetColor(out string? color) ? color : "white";
            _sb.AppendLine($"    {nodeLabel} [style=filled, fillcolor=\"{fill}\", color=\"black\"];");

            // Parent-child "contains" edges
            foreach (TaskNode child in node.Children)
            {
                string childLabel = GetLabel(child.Id, child.Name);
                _sb.AppendLine($"    {nodeLabel} -> {childLabel} [label=\"contains\"];");
            }

            // Dependency edges
            foreach (TaskNode dep in node.Dependencies)
            {
                string depLabel = GetLabel(dep.Id, dep.Name);
                _sb.AppendLine($"    {depLabel} -> {nodeLabel} [style=dashed, label=\"dependsOn\"];");
            }

            // Additional relations
            foreach (KeyValuePair<string, HashSet<TaskNode>> rel in node.Relations)
            {
                foreach (TaskNode target in rel.Value)
                {
                    string targetLabel = GetLabel(target.Id, target.Name);
                    _sb.AppendLine($"    {nodeLabel} -> {targetLabel} [label=\"{Escape(rel.Key)}\"];");
                }
            }
        }

        public void End()
        {
            _sb.AppendLine("}");
        }

        public string GetResult() => _sb.ToString();

        private static string GetLabel(string id, string name)
            => $"\"{Escape(id + ": " + name)}\"";

        private static string Escape(string text)
            => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
