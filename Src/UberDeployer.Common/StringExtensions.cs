using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Common
{
  public static class StringExtensions
  {
    private static readonly Regex _VariableRegex = new Regex("\\$\\{(?<VariableName>[^\\}]+)\\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string ExpandVariables(this string s, IDictionary<string, string> variables)
    {
      Guard.NotNullNorEmpty(s, "s");
      Guard.NotNull(variables, "variables");

      return
        _VariableRegex.Replace(
          s,
          match =>
          {
            string variableName = match.Groups["VariableName"].Value;

            if (!variables.ContainsKey(variableName))
            {
              throw new ArgumentException(string.Format("No value for variable '{0}'.", variableName), "variables");
            }

            return variables[variableName];
          });
    }
  }
}
