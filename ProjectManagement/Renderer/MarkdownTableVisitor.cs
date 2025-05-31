using ProjectManagement.Core;

using System.Text;

namespace ProjectManagement.Renderer
{
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
            if (node.IsMilestone)
            {
                return; 
            }

            // Ensure Description is never null
            string rawDesc = node.Description ?? string.Empty;

            // 1) Replace any newline/CR with "<br>"
            // 2) Escape any "|" so it won't break the Markdown table
            // 3) Trim leading/trailing whitespace
            string desc = EscapeForTable(rawDesc);

            // Tags: join by comma, then escape any "|" in tag names (unlikely but safe)
            string tags = string.Join(",",
                node.Tags.GetNonControlTags().Select(EscapeForTable)
            );

            // Relations: format "relName:taskId" for each relation, then escape
            var relItems = node.Relations
                .SelectMany(r => r.Value.Select(t => $"{r.Key}:{t.Id}"))
                .Select(item => EscapeForTable(item));

            string rels = string.Join(", ", relItems);

            string id = EscapeForTable(node.Id);
            string name = EscapeForTable(node.Name);

            _sb.AppendLine($"| {level} | {id} | {name} | {desc} | {tags} | {rels} |");
        }

        public void End() { }

        public string GetResult() => _sb.ToString();

        /// <summary>
        /// Escapes any characters in a cell that would break a Markdown table:
        /// - Replaces '|' with '\|'  
        /// - Converts newlines/CRs into '<br>'  
        /// - Trims leading/trailing whitespace  
        /// </summary>
        private static string EscapeForTable(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // 1) Normalize all CR/LF pairs, then split on '\n' to insert <br>
            var lines = text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n');

            // 2) Escape any '|' within each line
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("|", "\\|");
            }

            // 3) Join back with "<br>"
            string joined = string.Join("<br>", lines);

            // 4) Trim extra whitespace
            return joined.Trim();
        }
    }
}
