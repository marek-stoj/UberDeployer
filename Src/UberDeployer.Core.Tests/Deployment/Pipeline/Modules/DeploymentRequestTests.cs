using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      DateTime dateRequested = DateTime.Now;
      string requestIdentity = "identity";
      string projectName = "projectName";
      string targetEnvironmentName = "targetEnvironmentName";
      var deploymentRequest = new DeploymentRequest
                              {
                                DateRequested = dateRequested,
                                FinishedSuccessfully = true,
                                ProjectName = projectName,
                                RequesterIdentity = requestIdentity,
                                TargetEnvironmentName = targetEnvironmentName
                              };

      Assert.AreEqual(dateRequested, deploymentRequest.DateRequested);
      Assert.AreEqual(requestIdentity, deploymentRequest.RequesterIdentity);
      Assert.AreEqual(projectName, deploymentRequest.ProjectName);
      Assert.AreEqual(targetEnvironmentName, deploymentRequest.TargetEnvironmentName);
      Assert.IsTrue(deploymentRequest.FinishedSuccessfully);
    }
  }
}
