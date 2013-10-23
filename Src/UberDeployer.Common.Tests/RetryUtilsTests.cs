using System;
using System.Reflection;
using NUnit.Framework;

namespace UberDeployer.Common.Tests
{
  [TestFixture]
  public class RetryUtilsTests
  {
    [Test]
    public void RetryOnException_retries_on_a_derived_exception()
    {
      // arrange
      bool finished = false;
      int triesCount = 0;

      Action action =
        () =>
        {
          triesCount++;

          if (triesCount == 1)
          {
            throw new InvalidFilterCriteriaException();
          }
          else
          {
            finished = true;
          }
        };

      // act
      RetryUtils.RetryOnException(
        new[] { typeof(ApplicationException) },
        1,
        0,
        action);

      // assert
      Assert.IsTrue(finished);
      Assert.AreEqual(2, triesCount);
    }

    [Test]
    public void RetryOnException_retries_the_given_number_of_times()
    {
      // arrange
      bool finished = false;
      int triesCount = 0;

      Action action =
        () =>
        {
          triesCount++;

          if (triesCount <= 3)
          {
            throw new Exception();
          }
          else
          {
            finished = true;
          }
        };

      // act
      RetryUtils.RetryOnException(
        new[] { typeof(Exception) },
        3,
        0,
        action);

      // assert
      Assert.AreEqual(4, triesCount);
      Assert.IsTrue(finished);
    }

    [Test]
    public void RetryOnException_doesnt_retry_on_a_unexpected_exception()
    {
      // arrange
      int triesCount = 0;

      Action action =
        () =>
        {
          triesCount++;

          throw new ArgumentOutOfRangeException();
        };

      // act & assert
      Assert.Throws<ArgumentOutOfRangeException>(
        () =>
        {
          RetryUtils.RetryOnException(
            new[] { typeof(InvalidOperationException) },
            1,
            0,
            action);
        });

      Assert.AreEqual(1, triesCount);
    }
  }
}
