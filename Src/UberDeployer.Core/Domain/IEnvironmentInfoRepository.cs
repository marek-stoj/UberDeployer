using System.Collections.Generic;

namespace UberDeployer.Core.Domain
{
  public interface IEnvironmentInfoRepository
  {
    IEnumerable<EnvironmentInfo> GetAll();

    EnvironmentInfo FindByName(string environmentName);
  }
}
