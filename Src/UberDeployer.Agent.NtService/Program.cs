using System;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace UberDeployer.Agent.NtService
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      GlobalContext.Properties["applicationName"] = "UberDeployer.Agent.NtService";
      XmlConfigurator.Configure();

      if (args.Length == 1 && args[0] == "/console")
      {
        var serviceHostContainer =
          new UberDeployerAgentServiceHostContainer();

        serviceHostContainer.Start();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        serviceHostContainer.Stop();
      }
      else
      {
        var servicesToRun = new ServiceBase[] { new UberDeployerAgentNtService() };

        ServiceBase.Run(servicesToRun);
      }
    }
  }
}