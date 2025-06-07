using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class PlantUmlDependencyVisitorTests
{
    [TestMethod]
    public void PlantUmlDependencyVisitorGeneratesEdges()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        var b = g.AddTask("B", "Task B");
        g.AddDependency("A", "B");
        g.AddRelation("A", "B", "rel");
        var visitor = new PlantUmlDependencyVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "@startuml");
        StringAssert.Contains(result, "-->");
        StringAssert.Contains(result, "rel");
        StringAssert.Contains(result, "@enduml");
    }
}
