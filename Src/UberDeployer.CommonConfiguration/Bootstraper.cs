using System;
using System.Collections.Generic;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Castle.MicroKernel.Registration;
using UberDeployer.Core.Configuration;
using UberDeployer.Core.DataAccess.NHibernate;
using UberDeployer.Core.Domain;
using UberDeployer.Core.DataAccess.Xml;
using UberDeployer.Core.DataAccess;
using UberDeployer.Core.Management.Db;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Management.NtServices;
using System.IO;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployer.Core.Deployment.Pipeline;

namespace UberDeployer.CommonConfiguration
{
  public class Bootstraper
  {
    private const string ApplicationConfigPath = @"Data\ApplicationConfiguration.xml";
    private const string ProjectInfoPath = @"Data\ProjectInfos.xml";
    private const string EnvironmentInfoPath = @"Data\EnvironmentInfos.xml";

    private static readonly TimeSpan _NtServiceManagerOperationsTimeout = TimeSpan.FromMinutes(1);

    private static ISessionFactory _sessionFactory;

    public static void Bootstrap()
    {
      var container = ObjectFactory.Container;

      container.Register(
        Component.For<IApplicationConfiguration>()
          .UsingFactoryMethod(() => new XmlApplicationConfiguration(ApplicationConfigPath))
          .LifeStyle.Singleton,
        Component.For<IProjectInfoRepository>()
          .UsingFactoryMethod(() => new XmlProjectInfoRepository(ProjectInfoPath))
          .LifeStyle.Singleton,
        Component.For<IEnvironmentInfoRepository>()
          .UsingFactoryMethod(() => new XmlEnvironmentInfoRepository(EnvironmentInfoPath))
          .LifeStyle.Singleton);

      container.Register(
        Component.For<ITeamCityClient>()
          .UsingFactoryMethod(
            () =>
              {
                var appConfig = container.Resolve<IApplicationConfiguration>();

                return new TeamCityClient(
                  appConfig.TeamCityHostName,
                  appConfig.TeamCityPort,
                  appConfig.TeamCityUserName,
                  appConfig.TeamCityPassword);
              })
          .LifeStyle.Transient);

      container.Register(
        Component.For<IArtifactsRepository>()
          .UsingFactoryMethod(() => new TeamCityArtifactsRepository(container.Resolve<ITeamCityClient>()))
          .LifeStyle.Transient);

      container.Register(
        Component.For<IDeploymentRequestRepository>()
          .UsingFactoryMethod(() => new NHibernateDeploymentRequestRepository(SessionFactory))
          .LifeStyle.Transient);

      container.Register(
        Component.For<INtServiceManager>()
          .UsingFactoryMethod(
            () =>
              {
                var appConfig = container.Resolve<IApplicationConfiguration>();

                return
                  new ScExeBasedNtServiceManager(
                    appConfig.ScExePath,
                    _NtServiceManagerOperationsTimeout);
              })
          .LifeStyle.Transient);

      container.Register(
        Component.For<ITaskScheduler>()
         .ImplementedBy<TaskScheduler>()
         .LifeStyle.Transient);

      // TODO IMM HI: config?
      container.Register(
        Component.For<IMsDeploy>()
          .UsingFactoryMethod(() => new MsDeploy(Path.Combine(Environment.CurrentDirectory, "msdeploy.exe")))
          .LifeStyle.Transient);

      // TODO IMM HI: config?
      container.Register(
        Component.For<IIisManager>()
          .UsingFactoryMethod(() => new MsDeployBasedIisManager(container.Resolve<IMsDeploy>()))
          .LifeStyle.Transient);

      // TODO IMM HI: config?
      container.Register(
        Component.For<IDeploymentPipeline>()
          .UsingFactoryMethod(
            () =>
              {
                var deploymentRequestRepository = container.Resolve<IDeploymentRequestRepository>();
                var auditingModule = new AuditingModule(deploymentRequestRepository);
                var deploymentPipeline = new DeploymentPipeline();

                deploymentPipeline.AddModule(auditingModule);

                return deploymentPipeline;
              })
          .LifeStyle.Transient);

      container.Register(
        Component.For<IDbVersionProvider>()
          .UsingFactoryMethod(
            () =>
              {
                IEnumerable<DbVersionTableInfo> versionTableInfos =
                  new List<DbVersionTableInfo>
                    {
                      new DbVersionTableInfo
                        {
                          TableName = "VERSION",
                          ColumnName = "dbVersion"
                        },
                      new DbVersionTableInfo
                        {
                          TableName = "VERSIONHISTORY",
                          ColumnName = "DBLabel"
                        }
                    };

                return new DbVersionProvider(versionTableInfos);
              })
          .LifeStyle.Transient);

    }

    private static ISessionFactory CreateNHibernateSessionFactory()
    {
      string connectionString = ObjectFactory.Instance.CreateApplicationConfiguration().ConnectionString;

      FluentConfiguration fluentConfiguration =
        Fluently.Configure()
          .Database(
            MsSqlConfiguration.MsSql2008
              .ConnectionString(connectionString))
          .Mappings(mc => mc.FluentMappings.AddFromAssemblyOf<NHibernateRepository>());

      return fluentConfiguration.BuildSessionFactory();
    }

    private static ISessionFactory SessionFactory
    {
      get { return _sessionFactory ?? (_sessionFactory = CreateNHibernateSessionFactory()); }
    }
  }
}
