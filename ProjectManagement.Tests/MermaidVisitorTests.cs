using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class MermaidVisitorTests
{
    [TestMethod]
    public void MermaidVisitorGeneratesEdges()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        var b = g.AddTask("B", "Task B");
        g.AddDependency("A", "B");
        g.AddRelation("A", "B", "rel");
        var visitor = new MermaidVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "graph TD");
        StringAssert.Contains(result, "-->" );
        StringAssert.Contains(result, "rel");
    }
}
