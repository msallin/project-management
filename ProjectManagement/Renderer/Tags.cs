using System.Collections.Immutable;

namespace ProjectManagement.Renderer
{
    public static class TagsExtensions
    {
        private const string _colorTag = "color:";

        public static readonly ImmutableHashSet<string> ControlTags = [_colorTag];

        public static bool IsControlTag(this string tag) 
            => tag.StartsWith(_colorTag, StringComparison.OrdinalIgnoreCase);

        public static bool GetColor(this IEnumerable<string> tags, out string? color)
        {
            color = tags.FirstOrDefault(t => t.StartsWith(_colorTag))?[(_colorTag.Length)..];
            return color != null;
        }
    }
}
