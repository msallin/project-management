using System.Collections.Immutable;

namespace ProjectManagement.Renderer
{
    public static class TagsExtensions
    {
        private const string _colorTag = "color:";

        private const string _milestoneTag = "milestone:";

        public static readonly ImmutableHashSet<string> ControlTags = [_colorTag, _milestoneTag];

        public static bool IsControlTag(this string tag) 
            => ControlTags.Any(c => tag.StartsWith(c, StringComparison.OrdinalIgnoreCase));

        public static bool IsMilestone(this IEnumerable<string> tags, out string? name)
        {
            name = tags.FirstOrDefault(t => t.StartsWith(_milestoneTag))?[(_milestoneTag.Length)..];
            return name != null;
        }

        public static bool GetColor(this IEnumerable<string> tags, out string? color)
        {
            color = tags.FirstOrDefault(t => t.StartsWith(_colorTag))?[(_colorTag.Length)..];
            return color != null;
        }
    }
}
