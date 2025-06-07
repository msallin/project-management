using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;
using System.Linq;

namespace ProjectManagement.Tests;

[TestClass]
public class DotVisitorTests
{
    [TestMethod]
    public void DotVisitorGeneratesEdges()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        var b = g.AddTask("B", "Task B");
        g.AddChild("A", "B");
        g.AddDependency("A", "B");
        g.AddRelation("A", "B", "rel");
        var visitor = new DotVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "\"A: Task A\"");
        StringAssert.Contains(result, "contains");
        StringAssert.Contains(result, "dependsOn");
        StringAssert.Contains(result, "rel");
    }
}
