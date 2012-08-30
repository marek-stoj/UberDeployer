using System;
using NUnit.Framework;
using UberDeployer.Core.Deployment.Pipeline.Modules;

namespace UberDeployer.Core.Tests.Deployment.Pipeline.Modules
{
  [TestFixture]
  public class DeploymentRequestTests
  {
    [Test]
    public void Constructor_ReturnObjectWithIdSetToMinusOne()
    {
      var deploymentRequest = new DeploymentRequest();

      Assert.AreEqual(-1, deploymentRequest.Id);
    }

    [Test]
    public void AfterAssignValueToProperties_PropertiesReturnCorrectValue()
    {
      DateTime dateStarted = DateTime.UtcNow;
      string requestIdentity = "identity";
      string projectName = "projectName";
      string targetEnvironmentName = "targetEnvironmentName";

      var deploymentRequest =
        new DeploymentRequest
          {
            DateStarted = dateStarted,
            DateFinished = dateStarted,
            FinishedSuccessfully = true,
            ProjectName = projectName,
            RequesterIdentity = requestIdentity,
            TargetEnvironmentName = targetEnvironmentName
          };

      Assert.AreEqual(dateStarted, deploymentRequest.DateStarted);
      Assert.AreEqual(dateStarted, deploymentRequest.DateFinished);
      Assert.AreEqual(requestIdentity, deploymentRequest.RequesterIdentity);
      Assert.AreEqual(projectName, deploymentRequest.ProjectName);
      Assert.AreEqual(targetEnvironmentName, deploymentRequest.TargetEnvironmentName);
      Assert.IsTrue(deploymentRequest.FinishedSuccessfully);
    }
  }
}
