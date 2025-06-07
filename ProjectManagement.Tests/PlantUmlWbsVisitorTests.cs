using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class PlantUmlWbsVisitorTests
{
    [TestMethod]
    public void PlantUmlWbsVisitorSkipsMilestones()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        var b = g.AddTask("B", "Task B");
        b.AddTag("milestone:x");
        g.AddChild("A", "B");
        var visitor = new PlantUmlWbsVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "@startwbs");
        StringAssert.Contains(result, "Project");
        Assert.IsFalse(result.Contains("B Task B"));
        StringAssert.Contains(result, "@endwbs");
    }
}
