using System.Text.RegularExpressions;

using ProjectManagement.Core;

namespace ProjectManagement.Importer
{
    /// <summary>
    /// Imports a single markdown file where:
    ///  - #...   = WBS hierarchy (1, 1.1, 1.1.1, …)
    ///  - paragraphs → Description
    ///  - @(X) or @(X,Y,Z) → one or more dependencies on tasks X, Y, Z
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

            // Matches @(X) or @(X,Y,Z) or @(X, Y, Z), where each X, Y, Z is digits-and-dots (e.g. "1", "1.2", "2.3.4", etc.)
            // Group 1 will capture all of "1", or "1,2,3", or "1.2, 3.4, 5".
            var depRegex = new Regex(@"@\(\s*([0-9\.]+(?:\s*,\s*[0-9\.]+)*)\s*\)");

            // Matches tags like #someTagName (no whitespace before the #, but not inside a word)
            var tagRegex = new Regex(@"(?<!\S)#([A-Za-z0-9_:]+)");

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // 1) Heading detection: lines beginning with one or more '#' then a space, then text
                Match m = Regex.Match(line, @"^(#+)\s+(.*)$");
                if (m.Success)
                {
                    int level = m.Groups[1].Value.Length;
                    string rest = m.Groups[2].Value.Trim();
                    string id, title;

                    // 1a) If the heading starts with a numeric ID (1 or 1.2 or 1.2.3, possibly followed by a dot), parse it out
                    Match numM = Regex.Match(rest, @"^(\d+(?:\.\d+)*)(?:\.)?\s+(.*)$");
                    if (numM.Success)
                    {
                        id = numM.Groups[1].Value;           // e.g. "1" or "1.2.3"
                        title = numM.Groups[2].Value.Trim();  // e.g. "PSVP Entkopplung" (no leading "1.")

                        // Sync levelCounters with the segments in 'id'
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
                        // 1b) No numeric prefix → auto-generate "1", "1.1", "1.1.1", based on counters
                        int prevCount = levelCounters.TryGetValue(level, out int cnt) ? cnt : 0;
                        levelCounters[level] = prevCount + 1;

                        title = rest;
                        id = string.Join(".",
                                  Enumerable.Range(1, level)
                                            .Select(l => levelCounters[l].ToString()));
                    }

                    // Clear counters and headingStack entries for levels deeper than the current
                    foreach (int d in levelCounters.Keys.Where(l => l > level).ToList())
                    {
                        levelCounters.Remove(d);
                    }

                    foreach (int d in headingStack.Keys.Where(l => l > level).ToList())
                    {
                        headingStack.Remove(d);
                    }

                    // Create the TaskNode
                    current = graph.AddTask(id, title);
                    headingStack[level] = current;

                    // Link to its parent (if level > 1)
                    if (level > 1 && headingStack.TryGetValue(level - 1, out TaskNode? parent))
                    {
                        graph.AddChild(parent.Id, id);
                    }
                }
                else if (current != null)
                {
                    // 2) Collect dependencies: each @(...) block may contain one or more IDs, comma-separated
                    foreach (Match dm in depRegex.Matches(line))
                    {
                        // dm.Groups[1].Value is e.g. "1" or "1,2,3" or "1.2, 3.4"
                        string inside = dm.Groups[1].Value;
                        // Split on comma, trim each piece, and add as separate dependency
                        foreach (string rawId in inside.Split(','))
                        {
                            string fromId = rawId.Trim();
                            if (!string.IsNullOrEmpty(fromId))
                            {
                                pendingDeps.Add((fromId, current.Id));
                            }
                        }
                    }

                    // 3) Collect tags (#tagName)
                    foreach (Match tm in tagRegex.Matches(line))
                    {
                        current.AddTag(tm.Groups[1].Value);
                    }

                    // 4) Remove all @(...) and #tags from the line before appending to description
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

            // 5) Resolve all collected dependencies
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
