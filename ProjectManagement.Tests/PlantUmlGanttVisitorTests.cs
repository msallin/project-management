using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class PlantUmlGanttVisitorTests
{
    [TestMethod]
    public void PlantUmlGanttVisitorHandlesMilestones()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        var b = g.AddTask("B", "Task B");
        b.AddTag("milestone:M1");
        g.AddDependency("A", "B");
        var visitor = new PlantUmlGanttVisitor();

        // act
        string result = g.ExportTopologicalSort(visitor);

        // assert
        StringAssert.Contains(result, "@startgantt");
        StringAssert.Contains(result, "happens at");
        StringAssert.Contains(result, "@endgantt");
    }
}
