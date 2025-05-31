using System.Collections.Immutable;

namespace ProjectManagement.Core;

public class TagsCollection : HashSet<string>
{
    private const string _colorTag = "color:";

    private const string _milestoneTag = "milestone:";

    public static readonly ImmutableHashSet<string> ControlTags = [_colorTag, _milestoneTag];

    public IEnumerable<string> GetControlTags()
        => this.Where(IsControlTag);

    public IEnumerable<string> GetNonControlTags()
        => this.Where(t => !IsControlTag(t));

    private static bool IsControlTag(string tag)
        => ControlTags.Any(c => tag.StartsWith(c, StringComparison.OrdinalIgnoreCase));

    public bool IsMilestone(out string? name)
    {
        name = this.FirstOrDefault(t => t.StartsWith(_milestoneTag))?[_milestoneTag.Length..];
        return name != null;
    }

    public bool GetColor(out string? color)
    {
        color = this.FirstOrDefault(t => t.StartsWith(_colorTag))?[_colorTag.Length..];
        return color != null;
    }
}
