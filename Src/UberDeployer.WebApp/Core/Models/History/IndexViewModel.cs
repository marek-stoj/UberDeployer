using System.Collections.Generic;

namespace UberDeployer.WebApp.Core.Models.History
{
  public class IndexViewModel : BaseViewModel
  {
    private List<DeploymentRequestViewModel> _deploymentRequests = new List<DeploymentRequestViewModel>();

    public IndexViewModel()
    {
      CurrentAppPage = AppPage.History;
    }

    public List<DeploymentRequestViewModel> DeploymentRequests
    {
      get { return _deploymentRequests; }
      set { _deploymentRequests = value ?? new List<DeploymentRequestViewModel>(); }
    }
  }
}
