using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectManagement.Core;
using System.Linq;

namespace ProjectManagement.Tests;

[TestClass]
public class TagsCollectionTests
{
    [TestMethod]
    public void ControlAndNonControlTagsAreSeparated()
    {
        // arrange
        var tags = new TagsCollection { "color:blue", "milestone:review", "custom" };

        // act
        var control = tags.GetControlTags().ToList();
        var nonControl = tags.GetNonControlTags().ToList();
        var isMilestone = tags.IsMilestone(out var milestone);
        var hasColor = tags.GetColor(out var color);

        // assert
        CollectionAssert.AreEquivalent(new[] { "color:blue", "milestone:review" }, control);
        CollectionAssert.AreEquivalent(new[] { "custom" }, nonControl);
        Assert.IsTrue(isMilestone);
        Assert.AreEqual("review", milestone);
        Assert.IsTrue(hasColor);
        Assert.AreEqual("blue", color);
    }

    [TestMethod]
    public void NoMilestoneOrColorReturnsFalse()
    {
        // arrange
        var tags = new TagsCollection { "other" };

        // act
        var isMilestone = tags.IsMilestone(out _);
        var hasColor = tags.GetColor(out _);

        // assert
        Assert.IsFalse(isMilestone);
        Assert.IsFalse(hasColor);
    }
}
