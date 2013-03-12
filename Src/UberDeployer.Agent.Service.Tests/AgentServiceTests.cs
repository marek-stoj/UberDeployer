using System;
using System.Collections.Generic;
using System.ServiceModel;
using Moq;
using NUnit.Framework;
using UberDeployer.Agent.Proxy.Faults;
using UberDeployer.Agent.Service.Diagnostics;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;

namespace UberDeployer.Agent.Service.Tests
{
  [TestFixture]
  public class AgentServiceTests
  {
    private AgentService _agentService;
    private Mock<IDeploymentPipeline> _deploymentPipelineFake;
    private Mock<IDiagnosticMessagesLogger> _diagnositcMessagesLoggerFake;
    private Mock<IProjectInfoRepository> _projectInfoRepositoryFake;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<ITeamCityClient> _teamCityClientFake;
    private Mock<IDeploymentRequestRepository> _deploymentRequestRepositoryFake;

    [SetUp]
    public void SetUp()
    {
      _deploymentPipelineFake = new Mock<IDeploymentPipeline>();
      _diagnositcMessagesLoggerFake = new Mock<IDiagnosticMessagesLogger>();
      _projectInfoRepositoryFake = new Mock<IProjectInfoRepository>();
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>();
      _teamCityClientFake = new Mock<ITeamCityClient>();
      _deploymentRequestRepositoryFake = new Mock<IDeploymentRequestRepository>();

      _agentService = new AgentService(
        _deploymentPipelineFake.Object,
        _projectInfoRepositoryFake.Object,
        _environmentInfoRepositoryFake.Object,
        _teamCityClientFake.Object,
        _deploymentRequestRepositoryFake.Object,
        _diagnositcMessagesLoggerFake.Object);
    }

    [Test]
    public void GetWebMachinesNames_fails_when_environment_does_not_exists()
    {
      // arrange  
      const string notExistingEnvName = "env name";

      _environmentInfoRepositoryFake
        .Setup(x => x.GetByName(notExistingEnvName))
        .Returns((EnvironmentInfo)null);

      // act assert
      Assert.Throws<FaultException<EnvironmentNotFoundFault>>(
        () => _agentService.GetWebMachinesNames(notExistingEnvName));
    }

    [Test]
    public void GetWebMachinesNames_properly_gets_machines_names()
    {
      // arrange  
      var expectedWebMachinesNames = new List<string> { "machine1", "machine2" };
      const string environmentName = "env name";

      var environmentInfo = GetEnvironmentInfo(environmentName, expectedWebMachinesNames);

      _environmentInfoRepositoryFake
        .Setup(x => x.GetByName(environmentName))
        .Returns(environmentInfo);

      // act      
      List<string> webMachinesNames = _agentService.GetWebMachinesNames(environmentName);

      // assert
      Assert.AreEqual(expectedWebMachinesNames.Count, webMachinesNames.Count);

      foreach (var webMachinesName in expectedWebMachinesNames)
      {
        Assert.Contains(webMachinesName, webMachinesNames);
      }
    }

    [Test]
    public void GetWebMachinesNames_fails_on_null_or_empty_environment_name()
    {
      Assert.Throws<ArgumentException>(() => _agentService.GetWebMachinesNames(null));
      Assert.Throws<ArgumentException>(() => _agentService.GetWebMachinesNames(string.Empty));
    }

    private static EnvironmentInfo GetEnvironmentInfo(string environmentName, IEnumerable<string> expectedWebMachinesNames)
    {
      return new EnvironmentInfo(
        environmentName,
        "configurationTemplateName",
        "appServerMachineName",
        "failOverMachineName",
        expectedWebMachinesNames,
        "terminalServerMachineName",
        "databaseServerMachineName",
        "ntServiceDirPath",
        "webAppsBaseDirPath",
        "schedulerAppsBaseDirPath",
        "terminalAppsBaseDirPath",
        false,
        new[] { new EnvironmentUser("id", "user") },
        new[] { new ProjectToFailoverClusterGroupMapping("projectName", "groupName") });
    }
  }
}
