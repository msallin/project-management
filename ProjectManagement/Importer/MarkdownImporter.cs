using System.Text.RegularExpressions;

using ProjectManagement.Core;

namespace ProjectManagement.Importer
{
    /// <summary>
    /// Imports a single Markdown file where:
    ///  - #...   = WBS hierarchy (1, 1.1, 1.1.1, …)
    ///  - paragraphs → Description
    ///  - @(X,Y,…) → one or more dependencies on tasks X, Y, …
    ///  - #tag    → tag on the current task
    /// </summary>
    public class MarkdownImporter
    {
        public void Import(IEnumerable<string> directories, ProjectGraph graph)
        {
            foreach (string dir in directories)
            {
                foreach (string file in Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories))
                {
                    ImportFile(file, graph);
                }
            }
        }

        public void ImportFile(string path, ProjectGraph graph)
            => ImportLines(File.ReadLines(path), graph);

        private void ImportLines(IEnumerable<string> lines, ProjectGraph graph)
        {
            var headingStack = new Dictionary<int, TaskNode>();
            var levelCounters = new Dictionary<int, int>();
            var pendingDeps = new List<(string fromId, string toId)>();
            TaskNode? current = null;

            // Matches multi‐ID dependency patterns: @(1), @(1,2,3), @(1.1, 2.2,3.3), etc.
            var depRegex = new Regex(@"@\(\s*([0-9\.]+(?:\s*,\s*[0-9\.]+)*)\s*\)");
            // Matches tags anywhere (including headings), e.g. #tagName or #some_tag:subtag
            var tagRegex = new Regex(@"(?<!\S)#([A-Za-z0-9_:]+)");

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // 1) Detect Markdown heading lines: "### Heading Text..."
                Match m = Regex.Match(line, @"^(#+)\s+(.*)$");
                if (m.Success)
                {
                    int level = m.Groups[1].Value.Length;       // Number of '#' chars
                    string rest = m.Groups[2].Value.Trim();     // Everything after the "#"s and space

                    var headingTags = new List<string>();
                    foreach (Match tm in tagRegex.Matches(rest))
                    {
                        headingTags.Add(tm.Groups[1].Value);
                    }
                    // Remove all "#tagName" fragments from the heading text
                    string restWithoutTags = tagRegex.Replace(rest, "").Trim();

                    // Now parse out numeric ID prefix (if provided), using the cleaned‐up text
                    string id, title;
                    Match numM = Regex.Match(restWithoutTags, @"^(\d+(?:\.\d+)*)(?:\.)?\s+(.*)$");
                    if (numM.Success)
                    {
                        // Explicit numeric prefix exists (e.g. "1" or "1.2.3", possibly followed by a dot)
                        id = numM.Groups[1].Value;                // e.g. "1" or "1.2.3"
                        title = numM.Groups[2].Value.Trim();       // e.g. "Requirements Review" (no tags, no "1.")

                        // Update levelCounters based on the numeric segments
                        int[] segs = id
                                     .Split('.', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToArray();
                        for (int i = 1; i <= level; i++)
                        {
                            levelCounters[i] = (i <= segs.Length) ? segs[i - 1] : 0;
                        }
                    }
                    else
                    {
                        // No explicit numeric prefix → auto‐generate WBS ID from levelCounters
                        int prevCount = levelCounters.TryGetValue(level, out int cnt) ? cnt : 0;
                        levelCounters[level] = prevCount + 1;
                        title = restWithoutTags;   // entire heading text, minus tags
                        id = string.Join(".",
                                  Enumerable.Range(1, level)
                                            .Select(l => levelCounters[l].ToString()));
                    }

                    // Clear counters and headingStack entries for deeper levels
                    foreach (int d in levelCounters.Keys.Where(l => l > level).ToList())
                    {
                        levelCounters.Remove(d);
                    }

                    foreach (int d in headingStack.Keys.Where(l => l > level).ToList())
                    {
                        headingStack.Remove(d);
                    }

                    // Create the TaskNode (with the sanitized title, no tags in it)
                    current = graph.AddTask(id, title);
                    // Immediately apply any tags that were on the heading
                    foreach (string tag in headingTags)
                    {
                        current.AddTag(tag);
                    }

                    // Push this node into the headingStack so children can link to it
                    headingStack[level] = current;
                    if (level > 1 && headingStack.TryGetValue(level - 1, out TaskNode? parent))
                    {
                        graph.AddChild(parent.Id, id);
                    }
                }
                else if (current != null)
                {
                    // 2) Collect dependencies: each @(…) block may contain one or more IDs
                    foreach (Match dm in depRegex.Matches(line))
                    {
                        string inside = dm.Groups[1].Value; // e.g. "1", or "1,2,3", or "1.1, 2.2"
                        foreach (string rawId in inside.Split(','))
                        {
                            string fromId = rawId.Trim();
                            if (!string.IsNullOrEmpty(fromId))
                            {
                                pendingDeps.Add((fromId, current.Id));
                            }
                        }
                    }

                    // 3) Collect any tags in non‐heading lines
                    foreach (Match tm in tagRegex.Matches(line))
                    {
                        current.AddTag(tm.Groups[1].Value);
                    }

                    // 4) Remove all @(…) and #tagName from the line before appending to Description
                    string cleaned = depRegex.Replace(line, "");
                    cleaned = tagRegex.Replace(cleaned, "").Trim();
                    if (!string.IsNullOrEmpty(cleaned))
                    {
                        current.Description = string.IsNullOrEmpty(current.Description)
                            ? cleaned
                            : current.Description + "\n" + cleaned;
                    }
                }
            }

            // 5) Finally, wire up all collected dependencies
            foreach ((string fromId, string toId) in pendingDeps)
            {
                if (graph.TryGetTask(fromId, out _))
                {
                    graph.AddDependency(fromId, toId);
                }
                else
                {
                    throw new KeyNotFoundException($"Dependency reference '{fromId}' not found.");
                }
            }
        }
    }
}
