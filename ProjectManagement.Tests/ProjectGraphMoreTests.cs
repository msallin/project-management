using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ProjectManagement.Tests;

[TestClass]
public class ProjectGraphMoreTests
{
    private class CollectVisitor : IProjectVisitor, IResultProvider
    {
        public readonly List<(string id, int level)> Visits = new();
        public void Start() { }
        public void Visit(TaskNode node, int level) => Visits.Add((node.Id, level));
        public void End() { }
        public string GetResult() => string.Join(",", Visits.Select(v => v.id));
    }

    [TestMethod]
    public void AddTask_DuplicateIdThrows()
    {
        // arrange
        var graph = new ProjectGraph();
        graph.AddTask("A", "Task A");

        // act & assert
        Assert.ThrowsException<ArgumentException>(() => graph.AddTask("A", "X"));
    }

    [TestMethod]
    public void AddChildAndDependencyWork()
    {
        // arrange
        var graph = new ProjectGraph();
        var p = graph.AddTask("P", "Parent");
        var c = graph.AddTask("C", "Child");

        // act
        graph.AddChild("P", "C");
        graph.AddDependency("P", "C");

        // assert
        Assert.IsTrue(p.Children.Contains(c));
        Assert.IsTrue(c.Dependencies.Contains(p));
    }

    [TestMethod]
    public void AddRelationConnectsNodes()
    {
        // arrange
        var graph = new ProjectGraph();
        var a = graph.AddTask("A", "A");
        var b = graph.AddTask("B", "B");

        // act
        graph.AddRelation("A", "B", "rel");

        // assert
        Assert.IsTrue(a.Relations["rel"].Contains(b));
    }

    [TestMethod]
    public void TopologicalSortReturnsOrder()
    {
        // arrange
        var g = new ProjectGraph();
        g.AddTask("1", "A");
        g.AddTask("2", "B");
        g.AddTask("3", "C");
        g.AddDependency("1", "2");
        g.AddDependency("2", "3");

        // act
        var sorted = g.TopologicalSort();

        // assert
        CollectionAssert.AreEqual(new[] { "1", "2", "3" }, sorted.Select(n => n.Id).ToList());
    }

    [TestMethod]
    public void TryGetTaskReturnsCorrectly()
    {
        // arrange
        var g = new ProjectGraph();
        g.AddTask("1", "A");

        // act
        bool found = g.TryGetTask("1", out TaskNode? node);
        bool notFound = g.TryGetTask("x", out _);

        // assert
        Assert.IsTrue(found);
        Assert.IsNotNull(node);
        Assert.IsFalse(notFound);
    }

    [TestMethod]
    public void ExportUsesVisitor()
    {
        // arrange
        var g = new ProjectGraph();
        g.AddTask("1", "A");
        g.AddTask("2", "B");
        var visitor = new CollectVisitor();

        // act
        string result = g.Export(visitor);

        // assert
        Assert.AreEqual("1,2", result.Trim(','));
        Assert.AreEqual(2, visitor.Visits.Count);
    }

    [TestMethod]
    public void ExportTopologicalSortUsesVisitor()
    {
        // arrange
        var g = new ProjectGraph();
        g.AddTask("1", "A");
        g.AddTask("2", "B");
        g.AddDependency("1", "2");
        var visitor = new CollectVisitor();

        // act
        string result = g.ExportTopologicalSort(visitor);

        // assert
        Assert.AreEqual("1,2", result.Trim(','));
        Assert.AreEqual(2, visitor.Visits.Count);
    }
}
