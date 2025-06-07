using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class MarkdownTableVisitorTests
{
    [TestMethod]
    public void MarkdownTableVisitorFormatsRows()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A", "Line1\nLine2");
        var b = g.AddTask("B", "Task B");
        b.AddTag("milestone:x");
        var visitor = new MarkdownTableVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "| Level | ID | Name | Description");
        StringAssert.Contains(result, "Line1<br>Line2");
        Assert.IsFalse(result.Contains("Task B")); // milestone skipped
    }
}
