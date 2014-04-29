using System;
using System.Collections.Generic;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class DbProjectInfo : ProjectInfo
  {
    public DbProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string dbName, string databaseServerId)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      DbName = dbName;
      DatabaseServerId = databaseServerId;
    }

    public override ProjectType Type
    {
      get { return ProjectType.Db; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new DbInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployDbProjectDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateDbScriptRunnerFactory(),
          objectFactory.CreateDbVersionProvider(),
          objectFactory.CreateFileAdapter(),
          objectFactory.CreateZipFileAdapter());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      throw new NotSupportedException();
    }

    public override string GetMainAssemblyFileName()
    {
      throw new NotSupportedException();
    }

    public string DbName { get; private set; }

    public string DatabaseServerId { get; set; }
  }
}
