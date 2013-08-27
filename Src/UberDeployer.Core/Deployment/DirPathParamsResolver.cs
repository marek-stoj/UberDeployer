using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class DirPathParamsResolver : IDirPathParamsResolver
  {
    private const string _OrderNumberParam = "${order.number}";

    private const string _CurrentDateParam = "${current.date}";

    private const string _ProjectNameParam = "${project.name}";    

    /// <param name="dirPath">Can be empty.</param>
    /// <param name="projectName"></param>
    public string ResolveProjectName(string dirPath, string projectName)
    {
      Guard.NotNull(dirPath, "dirPath");
      Guard.NotNullNorEmpty(projectName, "projectName");

      return dirPath.Contains(_ProjectNameParam)
               ? dirPath.Replace(_ProjectNameParam, projectName)
               : dirPath;
    }

    public string ResolveCurrentDate(string dirPath, string dateFormat)
    {
      if (dirPath.Contains(_CurrentDateParam))
      {
        string currentDate =
          string.IsNullOrEmpty(dateFormat)
            ? DateTime.Now.ToShortDateString()
            : DateTime.Now.ToString(dateFormat);

        return dirPath.Replace(_CurrentDateParam, currentDate);
      }

      return dirPath;
    }

    public string ResolveOrderNumber(string packageDirPath)
    {
      CheckOrderNumberParam(packageDirPath);

      if (packageDirPath.Contains(_OrderNumberParam))
      {
        int order = -1;
        string resolvedPackageDirPath;

        do
        {
          order++;
          resolvedPackageDirPath = packageDirPath.Replace(_OrderNumberParam, string.Format("{0:D2}", order));
        } while (Directory.Exists(resolvedPackageDirPath));

        packageDirPath = resolvedPackageDirPath;
      }      

      return packageDirPath;
    }

    private static void CheckOrderNumberParam(string packageDirPath)
    {
      if (packageDirPath.Contains(_OrderNumberParam))
      {
        packageDirPath = packageDirPath.Remove(packageDirPath.IndexOf(_OrderNumberParam, StringComparison.Ordinal), _OrderNumberParam.Length);

        if (packageDirPath.Contains(_OrderNumberParam))
        {
          throw new DeploymentTaskException(string.Format("Parameter '{0}' can be used only once.", _OrderNumberParam));
        }
      }
    }
  }
}


