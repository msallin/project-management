using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Tests;

[TestClass]
public class MindmapTagVisitorTests
{
    [TestMethod]
    public void MindmapTagVisitorGroupsByTag()
    {
        // arrange
        var g = new ProjectGraph();
        var a = g.AddTask("A", "Task A");
        a.AddTag("tag1");
        var b = g.AddTask("B", "Task B");
        var visitor = new MindmapTagVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        StringAssert.Contains(result, "@startmindmap");
        StringAssert.Contains(result, "* tag1");
        StringAssert.Contains(result, "* Untagged");
        StringAssert.Contains(result, "@endmindmap");
    }
}
