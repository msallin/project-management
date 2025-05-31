using System.Text.RegularExpressions;

using ProjectManagement.Core;

namespace ProjectManagement.Importer
{
    /// <summary>
    /// Imports a single markdown file where:
    ///  - #...   = WBS hierarchy (1, 1.1, 1.1.1, …)
    ///  - paragraphs → Description
    ///  - @(X)    → dependency on task X
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

            var depRegex = new Regex(@"@\(([0-9\.]+)\)");
            var tagRegex = new Regex(@"(?<!\S)#([A-Za-z0-9_:]+)");

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Match Markdown headings:  "#" characters followed by a space and then the text.
                Match m = Regex.Match(line, @"^(#+)\s+(.*)$");
                if (m.Success)
                {
                    int level = m.Groups[1].Value.Length;
                    string rest = m.Groups[2].Value.Trim();
                    string id, title;

                    // If the heading text starts with a numeric ID (e.g. "1" or "1.2.3"), extract it.
                    Match numM = Regex.Match(rest, @"^(\d+(?:\.\d+)*)\s+(.*)$");
                    if (numM.Success)
                    {
                        // Explicit ID was provided in the heading.
                        id = numM.Groups[1].Value;
                        title = numM.Groups[2].Value.Trim(); // <-- Exclude the numeric prefix from the title

                        // Update the level counters based on the provided segments
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
                        // No explicit numeric ID in the heading: auto-generate based on levelCounters
                        int prev = levelCounters.TryGetValue(level, out int cnt) ? cnt : 0;
                        levelCounters[level] = prev + 1;
                        title = rest;
                        id = string.Join(".",
                                  Enumerable.Range(1, level)
                                            .Select(l => levelCounters[l].ToString()));
                    }

                    // Reset counters and stack entries for deeper levels than the current one
                    foreach (int d in levelCounters.Keys.Where(l => l > level).ToList())
                    {
                        levelCounters.Remove(d);
                    }
                    foreach (int d in headingStack.Keys.Where(l => l > level).ToList())
                    {
                        headingStack.Remove(d);
                    }

                    // Add the new TaskNode using the extracted ID and the cleaned-up title
                    current = graph.AddTask(id, title);
                    headingStack[level] = current;

                    // Link to its parent (if any)
                    if (level > 1 && headingStack.TryGetValue(level - 1, out TaskNode? parent))
                    {
                        graph.AddChild(parent.Id, id);
                    }
                }
                else if (current != null)
                {
                    // 1) Collect dependencies from lines (e.g. @(1.2.3))
                    foreach (Match dm in depRegex.Matches(line))
                    {
                        pendingDeps.Add((dm.Groups[1].Value, current.Id));
                    }

                    // 2) Collect tags (#tagName)
                    foreach (Match tm in tagRegex.Matches(line))
                    {
                        current.AddTag(tm.Groups[1].Value);
                    }

                    // 3) Clean out markers (dependencies and tags) before appending to description
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

            // Resolve all collected dependencies
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
