using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer;

/// <summary>
/// Visitor that runs critical-path analysis and emits a Markdown report.
/// </summary>
public class CriticalPathVisitor(ProjectGraph graph) : IProjectVisitor, IResultProvider
{
    private readonly ProjectGraph _graph = graph;
    private CriticalPathResult? _result;
    private readonly StringBuilder _sb = new();

    public void Start()
    {
        // nothing to do on per-node start
    }

    public void Visit(TaskNode node, int level)
    {
        // we don't need to do anything on each node visit,
        // since CP uses the full graph, not the WBS order
    }

    public void End()
    {
        // 1) run the analysis
        _result = AnalyzeCriticalPath();

        // 2) build Markdown table of results
        _sb.AppendLine("| ID | Name | Dur | ES | EF | LS | LF | Slack |");
        _sb.AppendLine("|---|---|---|---|---|---|---|---|");
        foreach (TaskNode? node in _result.ES.Keys.OrderBy(n => _result.ES[n]))
        {
            _sb.AppendLine(
                $"| {Escape(node.Id)} " +
                $"| {Escape(node.Name)} " +
                $"| {node.Duration:c} " +
                $"| {_result.ES[node]:c} " +
                $"| {_result.EF[node]:c} " +
                $"| {_result.LS[node]:c} " +
                $"| {_result.LF[node]:c} " +
                $"| {_result.Slack[node]:c} |");
        }

        // 3) append the critical-path sequence
        _sb.AppendLine();
        _sb.Append("**Critical Path**: ");
        _sb.Append(string.Join(" → ",
            _result.CriticalPath.Select(n => $"{Escape(n.Id)} ({Escape(n.Name)})")));
    }

    public string GetResult() => _sb.ToString();

    private static string Escape(string text)
        => text.Replace("|", "\\|").Replace("\\", "\\\\");

    public CriticalPathResult AnalyzeCriticalPath()
    {
        List<TaskNode> sorted = _graph.TopologicalSort();

        // Forward pass
        var ES = new Dictionary<TaskNode, TimeSpan>();
        var EF = new Dictionary<TaskNode, TimeSpan>();
        foreach (TaskNode node in sorted)
        {
            TimeSpan earliest = node.Dependencies.Any()
              ? node.Dependencies.Max(p => EF[p])
              : TimeSpan.Zero;
            ES[node] = earliest;
            EF[node] = earliest + node.Duration;
        }

        // Backward pass
        TimeSpan projectFinish = EF.Values.Max();
        var LS = new Dictionary<TaskNode, TimeSpan>();
        var LF = new Dictionary<TaskNode, TimeSpan>();
        foreach (TaskNode node in sorted.Reverse<TaskNode>())
        {
            IEnumerable<TaskNode> successors = sorted.Where(n => n.Dependencies.Contains(node));
            if (successors.Any())
            {
                LF[node] = successors.Min(s => LS[s]);
            }
            else
            {
                LF[node] = projectFinish;
            }

            LS[node] = LF[node] - node.Duration;
        }

        // Slack and extract critical path
        var slack = new Dictionary<TaskNode, TimeSpan>();
        foreach (TaskNode node in sorted)
        {
            slack[node] = LS[node] - ES[node];
        }

        var critical = new List<TaskNode>();
        TaskNode? cursor = sorted.FirstOrDefault(n => ES[n] == TimeSpan.Zero && slack[n] == TimeSpan.Zero);
        while (cursor != null)
        {
            critical.Add(cursor);
            cursor = sorted
                .Where(n => n.Dependencies.Contains(cursor) && slack[n] == TimeSpan.Zero)
                .OrderBy(n => ES[n])
                .FirstOrDefault();
        }

        return new CriticalPathResult
        {
            ES = ES,
            EF = EF,
            LS = LS,
            LF = LF,
            Slack = slack,
            CriticalPath = critical
        };
    }

    public class CriticalPathResult
    {
        public required Dictionary<TaskNode, TimeSpan> ES { get; set; }

        public required Dictionary<TaskNode, TimeSpan> EF { get; set; }

        public required Dictionary<TaskNode, TimeSpan> LS { get; set; }

        public required Dictionary<TaskNode, TimeSpan> LF { get; set; }

        public required Dictionary<TaskNode, TimeSpan> Slack { get; set; }

        public required List<TaskNode> CriticalPath { get; set; }
    }
}
