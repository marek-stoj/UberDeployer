using System;
using System.Collections.Generic;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class DbProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public DbProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string dbName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      DbName = dbName;
    }

    #endregion

    #region Overrides of ProjectInfo

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
        new MigrateDbDeploymentTask(
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateDbScriptRunnerFactory(),
          objectFactory.CreateDbVersionProvider());
    }

    public override IEnumerable<string> GetTargetFolders(EnvironmentInfo environmentInfo)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region Properties

    public string DbName { get; private set; }

    #endregion
  }
}
