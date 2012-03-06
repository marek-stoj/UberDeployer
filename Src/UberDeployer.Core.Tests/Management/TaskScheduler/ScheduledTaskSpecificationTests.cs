using System;
using NUnit.Framework;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Management.TaskScheduler
{
  [TestFixture]
  public class ScheduledTaskSpecificationTests
  {
    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenNameIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification(string.Empty, "exeAbsolutePath", 0, 0, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenExeAbsolutePathIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", string.Empty, 0, 0, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenExeAbsolutePathIsNotAbsolutePath_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "ItIsNotAnAbsolutePathJustBecauseItIsInEnglish", 0, 0, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenScheduledHourIsAfter23_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "c:\\", 24, 0, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenScheduledHourIsBefore0_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "c:\\", -1, 0, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenScheduledMinuteIsOver59_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "c:\\", 4, 60, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenScheduledMinuteIsLessThanZero_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "c:\\", 4, -1, 0));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenExecutionTimeLimitInMinutesIsLessThanZero_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScheduledTaskSpecification("name", "c:\\", 4, 4, -1));
    }

    [Test]
    public void ScheduledTaskSpecificationConstructor_WhenEverythinkIsOk_AllPropertiesHavaCorrectValues()
    {
      // Arrange
      string name = "name";
      string exeAbsolutePath = "c:\\";
      int scheduledHour = 4;
      int scheduledMinute = 5;
      int executionTimeLimitInMinutes = 6;

      // Act
      var scheduledTaskSpecification = new ScheduledTaskSpecification(name, exeAbsolutePath, scheduledHour, scheduledMinute, executionTimeLimitInMinutes);

      // Assert
      Assert.AreEqual(name, scheduledTaskSpecification.Name);
      Assert.AreEqual(exeAbsolutePath, scheduledTaskSpecification.ExeAbsolutePath);
      Assert.AreEqual(scheduledHour, scheduledTaskSpecification.ScheduledHour);
      Assert.AreEqual(scheduledMinute, scheduledTaskSpecification.ScheduledMinute);
      Assert.AreEqual(executionTimeLimitInMinutes, scheduledTaskSpecification.ExecutionTimeLimitInMinutes);
    }
  }
}
