namespace UberDeployer.Core.Management.NtServices
{
  public interface INtServiceManager
  {
    bool DoesServiceExist(string machineName, string serviceName);

    void InstallService(string machineName, NtServiceDescriptor ntServiceDescriptor);

    void UninstallService(string machineName, string serviceName);

    void StartService(string machineName, string serviceName);

    void StopService(string machineName, string serviceName);
  }
}
