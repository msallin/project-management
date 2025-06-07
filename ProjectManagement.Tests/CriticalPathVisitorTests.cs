using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;
using System.Linq;

namespace ProjectManagement.Tests;

[TestClass]
public class CriticalPathVisitorTests
{
    [TestMethod]
    public void CriticalPathVisitorComputesPath()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "A");
        var b = g.AddTask("B", "B");
        g.AddDependency("A", "B");
        var visitor = new CriticalPathVisitor(g);

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "Critical Path");
        var analysis = visitor.AnalyzeCriticalPath();
        CollectionAssert.AreEqual(new[] { a, b }, analysis.CriticalPath.ToArray());
    }
}
