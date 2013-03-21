using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class CreateWebDeployPackageDeploymentStep : DeploymentStep
  {
    private const string _WebDeployPackageFileName = "webDeployPackage.zip";
    private const string _WebDeployManifestFileName = "webDeployManifest.xml";

    private readonly IMsDeploy _msDeploy;
    private readonly Lazy<string> _webAppBinariesDirPathProvider;   
    private readonly string _iisSiteName;
    private readonly string _webAppName;
    
    private readonly string _fullWebAppName;

    private string _webAppBinariesParentDirPath
    {
      get 
      { 
        var webAppBinariesParentDirPath = Path.GetDirectoryName(_webAppBinariesDirPathProvider.Value);

        if (string.IsNullOrEmpty(webAppBinariesParentDirPath))
        {
          throw new ArgumentException(string.Format("Given web app binaries dir path ('{0}') is not valid because its parent can't be determined.", _webAppBinariesDirPathProvider));
        }
        
        return webAppBinariesParentDirPath;
      }
    }

    private string _packageFilePath
    {
      get { return Path.Combine(_webAppBinariesParentDirPath, _WebDeployPackageFileName); }
    }

    #region Constructor(s)

    public CreateWebDeployPackageDeploymentStep(IMsDeploy msDeploy, Lazy<string> webAppBinariesDirPathProvider, string iisSiteName, string webAppName = null)
    {
      Guard.NotNull(msDeploy, "msDeploy");      
      Guard.NotNull(webAppBinariesDirPathProvider, "webAppBinariesDirPathProvider");
      Guard.NotNullNorEmpty(iisSiteName, "iisSiteName");

      _msDeploy = msDeploy;
      _webAppBinariesDirPathProvider = webAppBinariesDirPathProvider;
      _iisSiteName = iisSiteName;
      _webAppName = webAppName;

      _fullWebAppName = _iisSiteName;

      if (!string.IsNullOrEmpty(_webAppName))
      {
        _fullWebAppName += string.Format("/{0}", _webAppName);
      }
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      if (!Path.IsPathRooted(_webAppBinariesDirPathProvider.Value))
      {
        throw new ArgumentException(string.Format("Given web app binaries dir path ('{0}') is not an absolute path.", _webAppBinariesDirPathProvider.Value));
      }

      string iisAppPath = _webAppBinariesDirPathProvider.Value;
      string webDeployManifestFilePath = Path.Combine(_webAppBinariesParentDirPath, _WebDeployManifestFileName);

      try
      {
        _msDeploy.CreateIisAppManifestFile(iisAppPath, webDeployManifestFilePath);

        string paramMatchValue =
          string.Format(
            "^{0}$",
            _webAppBinariesDirPathProvider.Value
              .Replace("\\", "\\\\")
              .Replace(".", "\\."));

        var msDeployArgs =
          new[]
            {
              "-verb:sync",
              string.Format("-source:manifest=\"{0}\"", webDeployManifestFilePath),
              string.Format("-dest:package=\"{0}\"", _packageFilePath),
              string.Format("-declareParam:name=IIS Web Application Name,defaultValue=\"{0}\",tags=iisApp", _fullWebAppName),
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
            _webAppBinariesDirPathProvider);
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
