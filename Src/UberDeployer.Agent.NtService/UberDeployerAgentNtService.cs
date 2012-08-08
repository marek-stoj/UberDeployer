using System.ServiceProcess;

namespace UberDeployer.Agent.NtService
{
  partial class UberDeployerAgentNtService : ServiceBase
  {
    private readonly UberDeployerAgentServiceHostContainer _serviceHostContainer;

    public UberDeployerAgentNtService()
    {
      InitializeComponent();

      _serviceHostContainer = new UberDeployerAgentServiceHostContainer();
    }

    protected override void OnStart(string[] args)
    {
      _serviceHostContainer.Start();
    }

    protected override void OnStop()
    {
      _serviceHostContainer.Stop();
    }
  }
}
