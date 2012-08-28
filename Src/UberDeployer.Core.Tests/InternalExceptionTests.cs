using System;
using NUnit.Framework;
using System.Linq;

namespace UberDeployer.Core.Tests
{
  [TestFixture]
  public class InternalExceptionTests
  {
    [Test]
    public void InternalExceptionConstrucotr_WhenMessageAndInnerExceptionIsCorrect_ReturnCorrectMessageAndInnerException()
    {
      string message = "Exception message";
      InvalidOperationException invalidOperationException = new InvalidOperationException();
      InternalException internalException = new InternalException(message, invalidOperationException);

      Assert.AreEqual(message, internalException.Message);
      Assert.AreEqual(invalidOperationException, internalException.InnerException);
    }

    [Test]
    public void InternalExceptionConstrucotr_WhenMessageIsCorrect_ReturnCorrectMessage()
    {
      string message = "Exception message";
      InternalException internalException = new InternalException(message);

      Assert.AreEqual(message, internalException.Message);
    }

    [Test]
    public void InternalExceptionConstrucotr_WhenMessageAndInnerExceptionIsIncorrect_DoesNotThrows()
    {
      Assert.DoesNotThrow(() => new InternalException(null, null));
    }
  }
}
