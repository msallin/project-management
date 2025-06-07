using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Importer;
using System.IO;

namespace ProjectManagement.Tests;

[TestClass]
public class MarkdownImporterTests
{
    [TestMethod]
    public void ImportFileParsesTasksAndDependencies()
    {
        // arrange
        var markdown = "# Task A #tag\n## Task B\n@(1)\nDescription";
        string path = Path.GetTempFileName();
        File.WriteAllText(path, markdown);
        var graph = new ProjectGraph();
        var importer = new MarkdownImporter();

        // act
        importer.ImportFile(path, graph);

        // assert
        Assert.IsTrue(graph.TryGetTask("1", out var a));
        Assert.IsTrue(graph.TryGetTask("1.1", out var b));
        Assert.IsTrue(a!.Tags.Contains("tag"));
        Assert.AreEqual("Description", b!.Description);
        Assert.IsTrue(b.Dependencies.Contains(a));

        File.Delete(path);
    }

    [TestMethod]
    public void ImportDirectoryParsesAllFiles()
    {
        // arrange
        var dir = Directory.CreateTempSubdirectory();
        string path = Path.Combine(dir.FullName, "test.md");
        File.WriteAllText(path, "# A\n");
        var graph = new ProjectGraph();
        var importer = new MarkdownImporter();

        // act
        importer.Import(new[] { dir.FullName }, graph);

        // assert
        Assert.IsTrue(graph.TryGetTask("1", out _));
        dir.Delete(true);
    }
}
