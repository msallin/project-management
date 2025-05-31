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

                Match m = Regex.Match(line, @"^(#+)\s+(.*)$");
                if (m.Success)
                {
                    int level = m.Groups[1].Value.Length;
                    string rest = m.Groups[2].Value.Trim();
                    string id, title;

                    Match numM = Regex.Match(rest, @"^(\d+(?:\.\d+)*)\s+(.*)$");
                    if (numM.Success)
                    {
                        int[] segs = numM.Groups[1].Value
                                     .Split('.', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToArray();

                        for (int i = 1; i <= level; i++)
                        {
                            levelCounters[i] = (i <= segs.Length) ? segs[i - 1] : 0;
                        }

                        title = numM.Groups[2].Value.Trim();
                        id = numM.Groups[1].Value;
                    }
                    else
                    {
                        int prev = levelCounters.TryGetValue(level, out int cnt) ? cnt : 0;
                        levelCounters[level] = prev + 1;
                        title = rest;
                        id = string.Join(".",
                                  Enumerable.Range(1, level)
                                            .Select(l => levelCounters[l].ToString()));
                    }

                    // reset deeper levels
                    foreach (int d in levelCounters.Keys.Where(l => l > level).ToList())
                    {
                        levelCounters.Remove(d);
                    }

                    foreach (int d in headingStack.Keys.Where(l => l > level).ToList())
                    {
                        headingStack.Remove(d);
                    }

                    current = graph.AddTask(id, title);
                    headingStack[level] = current;
                    if (level > 1 && headingStack.TryGetValue(level - 1, out TaskNode? parent))
                    {
                        graph.AddChild(parent.Id, id);
                    }
                }
                else if (current != null)
                {
                    // 1) collect dependencies
                    foreach (Match dm in depRegex.Matches(line))
                    {
                        pendingDeps.Add((dm.Groups[1].Value, current.Id));
                    }

                    // 2) collect tags
                    foreach (Match tm in tagRegex.Matches(line))
                    {
                        current.AddTag(tm.Groups[1].Value);
                    }

                    // 3) clean out markers from description
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

            // resolve dependencies
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
