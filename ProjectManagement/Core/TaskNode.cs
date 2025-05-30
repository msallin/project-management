namespace ProjectManagement.Core;

/// <summary>
///     Represents both WBS tree structure, dependency graph, and generic relations.
/// </summary>
public class TaskNode
{
    public string Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public TimeSpan Duration { get; set; } = TimeSpan.FromDays(1);

    public HashSet<string> Tags { get; } = [];

    public List<TaskNode> Dependencies { get; } = [];

    public List<TaskNode> Children { get; } = [];

    public Dictionary<string, HashSet<TaskNode>> Relations { get; } = [];

    public TaskNode(string id, string name, string description = "", IEnumerable<string>? tags = null)
    {
        Id = id;
        Name = name;
        Description = description;
        if (tags != null)
        {
            foreach (var tag in tags)
            {
                Tags.Add(tag);
            }
        }
    }

    public void AddTag(string value) => Tags.Add(value);

    public void AddRelation(string relationType, TaskNode target)
    {
        if (!Relations.TryGetValue(relationType, out var set))
        {
            set = [];
            Relations[relationType] = set;
        }
        set.Add(target);
    }

    public void Accept(IProjectVisitor visitor, int level = 1)
    {
        visitor.Visit(this, level);
        foreach (var child in Children.OrderBy(c => c.Id))
        {
            child.Accept(visitor, level + 1);
        }
    }
}
