using System.Collections.Generic;

namespace UberDeployer.Core.Domain
{
  public interface IProjectInfoRepository
  {
    IEnumerable<ProjectInfo> GetAll();

    ProjectInfo FindByName(string name);
  }
}
