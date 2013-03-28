﻿using UberDeployer.Common.IO;
using UberDeployer.Core;
using UberDeployer.Core.Configuration;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.Metadata;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployer.Core.TeamCity;
using Castle.Windsor;

namespace UberDeployer.CommonConfiguration
{
  public class ObjectFactory : IObjectFactory
  {
    private static WindsorContainer _container;
    private static IObjectFactory _instance;

    private static string _webAppInternalApiEndpointUrl;
    private static int _webAsynchronousPasswordCollectorMaxWaitTimeInSeconds;

    #region Constructor(s)

    private ObjectFactory()
    {
      // singleton
    }

    #endregion

    #region IObjectFactory members

    public IApplicationConfiguration CreateApplicationConfiguration()
    {
      return _container.Resolve<IApplicationConfiguration>();
    }

    public IProjectInfoRepository CreateProjectInfoRepository()
    {
      return _container.Resolve<IProjectInfoRepository>();
    }

    public IEnvironmentInfoRepository CreateEnvironmentInfoRepository()
    {
      return _container.Resolve<IEnvironmentInfoRepository>();
    }

    public IArtifactsRepository CreateArtifactsRepository()
    {
      return _container.Resolve<IArtifactsRepository>();
    }

    public IDeploymentRequestRepository CreateDeploymentRequestRepository()
    {
      return _container.Resolve<IDeploymentRequestRepository>();
    }

    public ITeamCityClient CreateTeamCityClient()
    {
      return _container.Resolve<ITeamCityClient>();
    }

    public INtServiceManager CreateNtServiceManager()
    {
      return _container.Resolve<INtServiceManager>();
    }

    public IMsDeploy CreateIMsDeploy()
    {
      return _container.Resolve<IMsDeploy>();
    }

    public IIisManager CreateIIisManager()
    {
      return _container.Resolve<IIisManager>();
    }

    public ITaskScheduler CreateTaskScheduler()
    {
      return _container.Resolve<ITaskScheduler>();
    }

    public IDeploymentPipeline CreateDeploymentPipeline()
    {
      return _container.Resolve<IDeploymentPipeline>();
    }

    public IPasswordCollector CreatePasswordCollector()
    {
      if (string.IsNullOrEmpty(_webAppInternalApiEndpointUrl))
      {
        IApplicationConfiguration applicationConfiguration =
          CreateApplicationConfiguration();

        _webAppInternalApiEndpointUrl = applicationConfiguration.WebAppInternalApiEndpointUrl;
        _webAsynchronousPasswordCollectorMaxWaitTimeInSeconds = applicationConfiguration.WebAsynchronousPasswordCollectorMaxWaitTimeInSeconds;
      }

      return
        new AsynchronousWebPasswordCollector(
          _webAppInternalApiEndpointUrl,
          _webAsynchronousPasswordCollectorMaxWaitTimeInSeconds);
    }

    public IDbScriptRunnerFactory CreateDbScriptRunnerFactory()
    {
      return new MsSqlDbScriptRunnerFactory();
    }

    public IDbVersionProvider CreateDbVersionProvider()
    {
      return _container.Resolve<IDbVersionProvider>();
    }

    public IFailoverClusterManager CreateFailoverClusterManager()
    {
      return new PowerShellFailoverClusterManager();
    }

    public IDirectoryAdapter CreateDirectoryAdapter()
    {
      return _container.Resolve<IDirectoryAdapter>();
    }

    public IProjectMetadataExplorer CreateProjectMetadataExplorer()
    {
      return new ProjectMetadataExplorer(this, CreateProjectInfoRepository(), CreateEnvironmentInfoRepository());
    }

    #endregion

    #region Properties

    public static IObjectFactory Instance
    {
      get { return (_instance ?? (_instance = new ObjectFactory())); }
    }

    public static WindsorContainer Container
    {
      get { return _container ?? (_container = new WindsorContainer()); }
    }

    #endregion
  }
}
