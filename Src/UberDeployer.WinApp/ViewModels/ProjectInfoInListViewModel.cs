using System;
using UberDeployer.Core;
using UberDeployer.Core.Domain;

namespace UberDeployer.WinApp.ViewModels
{
  [Serializable]
  public class ProjectInfoInListViewModel
  {
    public ProjectInfoInListViewModel(ProjectInfo projectInfo)
    {
      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      ProjectInfo = projectInfo;
    }

    public ProjectInfo ProjectInfo { get; private set; }

    public string Name
    {
      get { return ProjectInfo.Name; }
    }

    public string Type
    {
      get { return ProjectInfo.Type; }
    }
  }
}
