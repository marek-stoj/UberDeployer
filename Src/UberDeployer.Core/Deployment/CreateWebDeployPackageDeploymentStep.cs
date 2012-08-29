using System;
using System.IO;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class CreateWebDeployPackageDeploymentStep : DeploymentStep
  {
    private const string _WebDeployPackageFileName = "webDeployPackage.zip";
    private const string _WebDeployManifestFileName = "webDeployManifest.xml";

    private readonly IMsDeploy _msDeploy;
    private readonly string _webAppBinariesDirPath;
    private readonly string _iisSiteName;
    private readonly string _webApplicationName;

    private readonly string _webAppBinariesParentDirPath;
    private readonly string _packageFilePath;

    #region Constructor(s)

    public CreateWebDeployPackageDeploymentStep(IMsDeploy msDeploy, string webAppBinariesDirPath, string iisSiteName, string webApplicationName)
    {
      if (msDeploy == null)
      {
        throw new ArgumentNullException("msDeploy");
      }

      if (string.IsNullOrEmpty(webAppBinariesDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webAppBinariesDirPath");
      }

      if (!Path.IsPathRooted(webAppBinariesDirPath))
      {
        throw new ArgumentException(string.Format("Given web app binaries dir path ('{0}') is not an absolute path.", webAppBinariesDirPath), "webAppBinariesDirPath");
      }

      if (string.IsNullOrEmpty(iisSiteName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "iisSiteName");
      }

      if (string.IsNullOrEmpty(webApplicationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webApplicationName");
      }

      _msDeploy = msDeploy;
      _webAppBinariesDirPath = webAppBinariesDirPath;
      _iisSiteName = iisSiteName;
      _webApplicationName = webApplicationName;

      _webAppBinariesParentDirPath = Path.GetDirectoryName(_webAppBinariesDirPath);

      if (string.IsNullOrEmpty(_webAppBinariesParentDirPath))
      {
        throw new ArgumentException(string.Format("Given web app binaries dir path ('{0}') is not valid because its parent can't be determined.", webAppBinariesDirPath), "webAppBinariesDirPath");
      }

      _packageFilePath = Path.Combine(_webAppBinariesParentDirPath, _WebDeployPackageFileName);
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      string iisAppPath = _webAppBinariesDirPath;
      string webDeployManifestFilePath = Path.Combine(_webAppBinariesParentDirPath, _WebDeployManifestFileName);

      try
      {
        _msDeploy.CreateIisAppManifestFile(iisAppPath, webDeployManifestFilePath);

        string paramMatchValue =
          string.Format(
            "^{0}$",
            _webAppBinariesDirPath
              .Replace("\\", "\\\\")
              .Replace(".", "\\."));

        string _fullWebApplicationName = string.Format("{0}/{1}", _iisSiteName, _webApplicationName);

        var msDeployArgs =
          new[]
            {
              "-verb:sync",
              string.Format("-source:manifest=\"{0}\"", webDeployManifestFilePath),
              string.Format("-dest:package=\"{0}\"", _packageFilePath),
              string.Format("-declareParam:name=IIS Web Application Name,defaultValue=\"{0}\",tags=iisApp", _fullWebApplicationName),
              string.Format("-declareParam:name=IIS Web Application Name,kind=ProviderPath,scope=iisApp,match=\"{0}\"", paramMatchValue),
              string.Format("-declareParam:name=IIS Web Application Name,kind=ProviderPath,scope=setAcl,match=\"{0}\"", paramMatchValue),
            };

        string consoleOutput;

        _msDeploy.Run(msDeployArgs, out consoleOutput);
      }
      finally
      {
        if (File.Exists(webDeployManifestFilePath))
        {
          File.Delete(webDeployManifestFilePath);
        }
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Create WebDeploy package ('{0}') using web app binaries from '{1}'.",
            _packageFilePath,
            _webAppBinariesDirPath);
      }
    }

    #endregion

    #region Properties

    public string PackageFilePath
    {
      get { return _packageFilePath; }
    }

    #endregion
  }
}
