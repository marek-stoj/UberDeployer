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
      _environmentInfoRepositoryFake =new Mock<IEnvironmentInfoRepository>();
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
    public void GetWebMachineNames_fails_when_environment_does_not_exists()
    {
      // arrange  
      const string notExistingEnvName = "env name";
      
      _environmentInfoRepositoryFake
        .Setup(x => x.GetByName(notExistingEnvName))
        .Returns((EnvironmentInfo) null);

      // act assert
      Assert.Throws<FaultException<EnvironmentNotFoundFault>>(
        () => _agentService.GetWebMachineNames(notExistingEnvName));
    }

    [Test]
    public void GetWebMachineNames_properly_gets_machines_names()
    {
      // arrange  
      var expectedWebMachineNames = new List<string> {"machine1", "machine2"};
      const string environmentName = "env name";

      var environmentInfo = GetEnvironmentInfo(environmentName, expectedWebMachineNames);

      _environmentInfoRepositoryFake
        .Setup(x => x.GetByName(environmentName))
        .Returns(environmentInfo);

      // act      
      List<string> webMachineNames = _agentService.GetWebMachineNames(environmentName);

      // assert
      Assert.AreEqual(expectedWebMachineNames.Count, webMachineNames.Count);

      foreach (var webMachineName in expectedWebMachineNames)
      {
        Assert.Contains(webMachineName, webMachineNames);
      }
    }
    
    [Test]
    public void GetWebMachineNames_fails_on_null_or_empty_environment_name()
    {
      Assert.Throws<ArgumentException>(() => _agentService.GetWebMachineNames(null));
      Assert.Throws<ArgumentException>(() => _agentService.GetWebMachineNames(string.Empty));
    }

    private static EnvironmentInfo GetEnvironmentInfo(string environmentName, IEnumerable<string> expectedWebMachineNames)
    {
      return new EnvironmentInfo(
        environmentName,
        "configurationTemplateName",
        "appServerMachineName", 
        "failOverMachineName", 
        expectedWebMachineNames, 
        "terminalServerMachineName",
        "databaseServerMachineName", 
        "ntServiceDirPath", 
        "webAppsBaseDirPath", 
        "schedulerAppsBaseDirPath",
        "terminalAppsBaseDirPath", 
        false, 
        new[] {new EnvironmentUser("id", "user")},
        new[] {new ProjectToFailoverClusterGroupMapping("projectName", "groupName")});
    }
  }
}
