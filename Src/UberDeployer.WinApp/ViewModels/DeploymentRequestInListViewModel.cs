using System;
using UberDeployer.Agent.Proxy.Dto;

namespace UberDeployer.WinApp.ViewModels
{
  public class DeploymentRequestInListViewModel
  {
    private readonly DeploymentRequest _deploymentRequest;

    public DeploymentRequestInListViewModel(DeploymentRequest deploymentRequest)
    {
      if (deploymentRequest == null)
      {
        throw new ArgumentNullException("deploymentRequest");
      }

      _deploymentRequest = deploymentRequest;
    }

    public string Project
    {
      get { return _deploymentRequest.ProjectName; }
    }

    public string Environment
    {
      get { return _deploymentRequest.TargetEnvironmentName; }
    }

    public string Requester
    {
      get { return _deploymentRequest.RequesterIdentity; }
    }

    public string Date
    {
      get { return _deploymentRequest.DateFinished.ToString(); }
    }

    public string Successful
    {
      get { return _deploymentRequest.FinishedSuccessfully ? "YES" : "NO"; }
    }
  }
}
