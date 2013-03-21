using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentTask : DeploymentTaskBase
  {
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IEnvironmentInfoRepository _environmentInfoRepository;

    private readonly List<DeploymentTaskBase> _subTasks;

    private DeploymentInfo _deploymentInfo;
    private string _tempDirPath;

    #region Constructor(s)

    protected DeploymentTask(IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository)
    {
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(environmentInfoRepository, "environmentInfoRepository");

      _projectInfoRepository = projectInfoRepository;
      _environmentInfoRepository = environmentInfoRepository;

      _subTasks = new List<DeploymentTaskBase>();
    }

    #endregion

    #region Public methods

    public void Initialize(DeploymentInfo deploymentInfo)
    {
      Guard.NotNull(deploymentInfo, "deploymentInfo");

      _deploymentInfo = deploymentInfo;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      // do nothing
    }

    protected override void DoExecute()
    {
      try
      {
        if (!DeploymentInfo.IsSimulation)
        {
          PostDiagnosticMessage(string.Format("Executing: {0}", Description), DiagnosticMessageType.Info);
        }

        foreach (DeploymentTaskBase subTask in _subTasks)
        {
          var deploymentTask = subTask as DeploymentTask;

          if (deploymentTask != null)
          {
            deploymentTask.Initialize(DeploymentInfo);
          }

          subTask.Prepare();

          if (!DeploymentInfo.IsSimulation)
          {
            subTask.Execute();
          }
        }
      }
      finally
      {
        DeleteTemporaryDirectoryIfNeeded();
      }
    }

    public override string Description
    {
      get { return string.Join(Environment.NewLine, _subTasks.Select(st => st.Description).ToArray()); }
    }

    #endregion

    #region Protected members

    protected EnvironmentInfo GetEnvironmentInfo()
    {
      EnvironmentInfo environmentInfo =
        _environmentInfoRepository.FindByName(
          DeploymentInfo.TargetEnvironmentName);

      if (environmentInfo == null)
      {
        throw new DeploymentTaskException(string.Format("Environment named '{0}' doesn't exist.", DeploymentInfo.TargetEnvironmentName));
      }

      return environmentInfo;
    }

    protected T GetProjectInfo<T>()
      where T : ProjectInfo
    {
      ProjectInfo projectInfo =
        _projectInfoRepository.FindByName(DeploymentInfo.ProjectName);

      if (projectInfo == null)
      {
        throw new DeploymentTaskException(string.Format("Project named '{0}' doesn't exist.", DeploymentInfo.ProjectName));
      }

      if (!(projectInfo is T))
      {
        throw new DeploymentTaskException(string.Format("Project named '{0}' is not of the expected type: '{1}'.", DeploymentInfo.ProjectName, typeof(T).FullName));
      }

      return (T)projectInfo;
    }

    protected void AddSubTask(DeploymentTaskBase subTask)
    {
      if (subTask == null)
      {
        throw new ArgumentNullException("subTask");
      }

      _subTasks.Add(subTask);

      // this will cause the events raised by sub-tasks to bubble up
      subTask.DiagnosticMessagePosted += OnDiagnosticMessagePosted;
    }

    protected string GetTempDirPath()
    {
      if (string.IsNullOrEmpty(_tempDirPath))
      {
        string tempDirName = Guid.NewGuid().ToString("N");

        _tempDirPath = Path.Combine(Path.GetTempPath(), tempDirName);

        Directory.CreateDirectory(_tempDirPath);
      }

      return _tempDirPath;
    }

    protected DeploymentInfo DeploymentInfo
    {
      get
      {
        if (_deploymentInfo == null)
        {
          throw new InvalidOperationException("DeploymentInfo is missing - have you initialized the task?");
        }

        return _deploymentInfo;
      }
    }

    #endregion

    #region Private methods

    private void DeleteTemporaryDirectoryIfNeeded()
    {
      if (!string.IsNullOrEmpty(_tempDirPath) && Directory.Exists(_tempDirPath))
      {
        Directory.Delete(_tempDirPath, true);
      }
    }

    #endregion

    #region Properties

    public IEnumerable<DeploymentTaskBase> SubTasks
    {
      get { return _subTasks.AsReadOnly(); }
    }

    #endregion
  }
}
