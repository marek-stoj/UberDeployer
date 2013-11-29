using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;

namespace UberDeployer.Core.Tests.TestUtils
{
  public static class ReflectionTestTools
  {
    public static T CreateInstance<T>(OrderedDictionary defaultParams, string nullParamName)
    {
      ConstructorInfo constructor = typeof(T).GetConstructors()[0];
      object[] parameters = new object[defaultParams.Count];

      int i = 0;

      foreach (DictionaryEntry param in defaultParams)
      {
        if ((string)param.Key == nullParamName)
        {
          parameters[i] = null;
        }
        else
        {
          parameters[i] = param.Value;
        }

        i++;
      }

      try
      {
        return (T)constructor.Invoke(parameters);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException ?? e;
      }
    }

    public static void SetPrivatePropertyValue(object obj, string propertyName, object value)
    {
      obj.GetType()
        .GetProperty(propertyName)
        .SetValue(obj, value, BindingFlags.NonPublic, Type.DefaultBinder, null, CultureInfo.InvariantCulture);
    }
  }
}
