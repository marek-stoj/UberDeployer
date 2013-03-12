using System;

namespace UberDeployer.Common.SyntaxSugar
{
  public static class Guard
  {
    private const string _UnknownParamName = "(unknown)";

    public static void NotNull<T>(T obj, string parameterName = null)
      where T : class
    {
      if (obj == null)
      {
        throw new ArgumentNullException(parameterName ?? _UnknownParamName);
      }
    }

    public static void NotNullNorEmpty(string s, string paramName = null)
    {
      if (string.IsNullOrEmpty(s))
      {
        throw new ArgumentException("Argument can't be null nor empty.", paramName ?? _UnknownParamName);
      }
    }

    public static void NotEmpty(Guid guid, string paramName = null)
    {
      if (guid == Guid.Empty)
      {
        throw new ArgumentException("Guid argument can't be empty.", paramName ?? _UnknownParamName);
      }
    }
  }
}
