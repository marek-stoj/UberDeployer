using System;
using System.ServiceProcess;

namespace UberDeployer.Core.Management.NtServices
{
  public class NtServiceDescriptor
  {
    #region Constructor(s)

    public NtServiceDescriptor(string serviceName, string serviceExecutablePath, ServiceAccount serviceAccount, ServiceStartMode serviceStartMode, string serviceDisplayName = null, string serviceUserName = null, string servicePassword = null)
    {
      if (string.IsNullOrEmpty(serviceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceName");
      }

      if (string.IsNullOrEmpty(serviceExecutablePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceExecutablePath");
      }

      ServiceName = serviceName;
      ServiceExecutablePath = serviceExecutablePath;
      ServiceAccount = serviceAccount;
      ServiceStartMode = serviceStartMode;
      ServiceDisplayName = serviceDisplayName ?? serviceName;
      ServiceUserName = serviceUserName;
      ServicePassword = servicePassword;
    }

    #endregion

    #region Properties

    public string ServiceName { get; private set; }

    public string ServiceExecutablePath { get; private set; }

    public ServiceAccount ServiceAccount { get; private set; }

    public ServiceStartMode ServiceStartMode { get; private set; }

    public string ServiceDisplayName { get; private set; }

    public string ServiceUserName { get; private set; }

    public string ServicePassword { get; private set; }

    #endregion
  }
}
