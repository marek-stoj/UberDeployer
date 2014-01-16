using System;
using System.Reflection;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Common
{
  public static class ReflectionUtils
  {
    public static string GetCodeBaseFilePath(Assembly assembly)
    {
      Guard.NotNull(assembly, "assembly");

      string codeBase = assembly.CodeBase;
      UriBuilder uri = new UriBuilder(codeBase);
      
      return Uri.UnescapeDataString(uri.Path);
    }
  }
}
