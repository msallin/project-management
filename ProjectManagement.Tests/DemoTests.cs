using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement;

namespace ProjectManagement.Tests;

[TestClass]
public class DemoTests
{
    [TestMethod]
    public void LoadFromCodeProducesGraph()
    {
        // arrange / act
        var graph = Demo.LoadFromCode();

        // assert
        Assert.IsTrue(graph.TryGetTask("1", out _));
    }

    [TestMethod]
    public void LoadFromMarkdownProducesGraph()
    {
        // arrange / act
        var graph = Demo.LoadFromMarkdown();

        // assert
        Assert.IsTrue(graph.TryGetTask("1", out _));
    }
}
