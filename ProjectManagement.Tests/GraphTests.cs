using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;

using System;

namespace ProjectManagement.Tests;

[TestClass]
public class GraphTests
{
    [TestMethod]
    public void AddingReverseDependencyCreatesCycle()
    {
        var graph = new ProjectGraph();
        graph.AddTask("A", "Task A");
        graph.AddTask("B", "Task B");
        graph.AddDependency("A", "B");

        Assert.ThrowsException<InvalidOperationException>(() => graph.AddDependency("B", "A"));
    }
}
