using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using System.Collections.Generic;

namespace ProjectManagement.Tests;

[TestClass]
public class TaskNodeTests
{
    private class RecordingVisitor : IProjectVisitor
    {
        public readonly List<(TaskNode node, int level)> Visits = new();
        public void Start() { }
        public void Visit(TaskNode node, int level) => Visits.Add((node, level));
        public void End() { }
    }

    [TestMethod]
    public void AddTagAndRelationWorks()
    {
        // arrange
        var node = new TaskNode("1", "Task");
        var target = new TaskNode("2", "Target");

        // act
        node.AddTag("x");
        node.AddRelation("rel", target);

        // assert
        Assert.IsTrue(node.Tags.Contains("x"));
        Assert.IsTrue(node.Relations["rel"].Contains(target));
    }

    [TestMethod]
    public void AcceptVisitsHierarchy()
    {
        // arrange
        var root = new TaskNode("1", "Root");
        var child = new TaskNode("1.1", "Child");
        root.Children.Add(child);
        var visitor = new RecordingVisitor();

        // act
        root.Accept(visitor);

        // assert
        Assert.AreEqual(2, visitor.Visits.Count);
        Assert.AreEqual(root, visitor.Visits[0].node);
        Assert.AreEqual(child, visitor.Visits[1].node);
        Assert.AreEqual(1, visitor.Visits[0].level);
        Assert.AreEqual(2, visitor.Visits[1].level);
    }

    [TestMethod]
    public void IsMilestoneReflectsTag()
    {
        // arrange
        var node = new TaskNode("1", "M");
        node.AddTag("milestone:M1");

        // act
        bool result = node.IsMilestone;

        // assert
        Assert.IsTrue(result);
    }
}
