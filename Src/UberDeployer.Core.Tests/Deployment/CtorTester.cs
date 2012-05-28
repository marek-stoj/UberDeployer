using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace UberDeployer.Core.Tests.Deployment
{
  // TODO IMM HI: generalize; make sequential; move to KRD.Testing
  public class CtorTester<T>
  {
    private readonly List<object> _argumentsList;

    public CtorTester(IEnumerable<object> arguments)
    {
      if (arguments == null)
      {
        throw new ArgumentNullException("arguments");
      }

      _argumentsList = new List<object>(arguments);
    }

    public void TestAll()
    {
      for (int i = 0; i < _argumentsList.Count; i++)
      {
        TestSingle(i);
      }
    }

    public void TestSingle(int argumentToSkipIndex)
    {
      Type[] constructorArgumentsTypes = _argumentsList.Select(o => o.GetType()).ToArray();
      ConstructorInfo constructorInfo = typeof(T).GetConstructor(constructorArgumentsTypes);

      if (constructorInfo == null)
      {
        throw new InvalidOperationException("No constructor matching given arguments' types found!");
      }

      object[] arguments = new object[_argumentsList.Count];

      for (int i = 0; i < _argumentsList.Count; i++)
      {
        if (i == argumentToSkipIndex)
        {
          // skip one argument
          continue;
        }

        arguments[i] = _argumentsList[i];
      }

      try
      {
        constructorInfo.Invoke(arguments);

        Assert.Fail("An exception was expected.");
      }
      catch (TargetInvocationException exc)
      {
        Exception innerException = exc.InnerException;

        if (constructorArgumentsTypes[argumentToSkipIndex] == typeof(string))
        {
          Assert.IsInstanceOf<ArgumentException>(innerException);
        }
        else
        {
          Assert.IsInstanceOf<ArgumentNullException>(innerException);
        }
      }
    }
  }
}
