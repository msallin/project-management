namespace ProjectManagement.Core;

public partial class ProjectGraph
{
    private readonly Dictionary<string, TaskNode> _nodes = [];

    private readonly HashSet<TaskNode> _roots = [];

    public TaskNode AddTask(string id, string name, string description = "", IEnumerable<string>? tags = null)
    {
        if (_nodes.ContainsKey(id))
        {
            throw new ArgumentException($"Task with id '{id}' already exists.");
        }

        var node = new TaskNode(id, name, description, tags);

        // List of all nodes, ensure that we don't have duplicates
        _nodes[id] = node;

        // New task starts life as a root
        _roots.Add(node);

        return node;
    }

    public void AddChild(string parentId, string childId)
    {
        if (!_nodes.TryGetValue(parentId, out TaskNode? parent) || !_nodes.TryGetValue(childId, out TaskNode? child))
        {
            throw new KeyNotFoundException("Parent or child task not found.");
        }

        parent.Children.Add(child);

        // Once a node becomes someone’s child, it’s no longer a root
        _roots.Remove(child);
    }

    public void AddDependency(string fromId, string toId)
    {
        EnsureNodeExists(fromId);
        EnsureNodeExists(toId);
        var from = _nodes[fromId];
        var to = _nodes[toId];
        if (CreatesCycle(from, to))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        to.Dependencies.Add(from);
    }

    public void AddRelation(string fromId, string toId, string relationType)
    {
        EnsureNodeExists(fromId);
        EnsureNodeExists(toId);
        _nodes[fromId].AddRelation(relationType, _nodes[toId]);
    }

    private void EnsureNodeExists(string id)
    {
        if (!_nodes.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Task with id '{id}' not found.");
        }
    }

    private static bool CreatesCycle(TaskNode start, TaskNode target)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<TaskNode>();
        stack.Push(start);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node.Id == target.Id)
            {
                return true;
            }

            foreach (var dep in node.Dependencies)
            {
                if (visited.Add(dep.Id))
                {
                    stack.Push(dep);
                }
            }
        }
        return false;
    }

    public List<TaskNode> TopologicalSort()
    {
        var inDegree = _nodes.Values.ToDictionary(n => n, n => n.Dependencies.Count);
        var queue = new Queue<TaskNode>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var sorted = new List<TaskNode>();
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            sorted.Add(node);
            foreach (var child in _nodes.Values.Where(n => n.Dependencies.Contains(node)))
            {
                inDegree[child]--;
                if (inDegree[child] == 0)
                {
                    queue.Enqueue(child);
                }
            }
        }
        if (sorted.Count != _nodes.Count)
        {
            throw new InvalidOperationException("Graph contains at least one cycle.");
        }

        return sorted;
    }
    public bool TryGetTask(string id, out TaskNode? node)
        => _nodes.TryGetValue(id, out node);

    public string Export(IProjectVisitor visitor)
    {
        visitor.Start();

        // Iterate only over maintained root set
        foreach (var root in _roots.OrderBy(n => n.Id))
        {
            root.Accept(visitor);
        }

        visitor.End();
        return visitor is IResultProvider p ? p.GetResult() : string.Empty;
    }

    public string ExportTopologicalSort(IProjectVisitor visitor)
    {
        visitor.Start();
        foreach (var node in TopologicalSort())
        {
            visitor.Visit(node, 0);
        }

        visitor.End();
        return visitor is IResultProvider p ? p.GetResult() : string.Empty;
    }
}
