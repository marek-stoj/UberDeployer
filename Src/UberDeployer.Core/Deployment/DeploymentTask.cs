using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentTask : DeploymentTaskBase
  {
    protected readonly IEnvironmentInfoRepository _environmentInfoRepository;

    private readonly List<DeploymentTaskBase> _subTasks;

    private string _tempDirPath;

    #region Constructor(s)

    protected DeploymentTask(IEnvironmentInfoRepository environmentInfoRepository)
    {
      if (environmentInfoRepository == null)
      {
        throw new ArgumentNullException("environmentInfoRepository");
      }

      _environmentInfoRepository = environmentInfoRepository;

      _subTasks = new List<DeploymentTaskBase>();
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      try
      {
        foreach (DeploymentTaskBase subTask in _subTasks)
        {
          subTask.Prepare(DeploymentInfo);
          subTask.Execute();
        }

        PostDiagnosticMessage(string.Format("Finished '{0}' (\"{1}\").", GetType().Name, Description));
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

    protected EnvironmentInfo GetEnvironmentInfo()
    {
      EnvironmentInfo environmentInfo = _environmentInfoRepository.FindByName(DeploymentInfo.TargetEnvironmentName);

      if (environmentInfo == null)
      {
        throw new DeploymentTaskException(string.Format("Environment named '{0}' doesn't exist.", DeploymentInfo.TargetEnvironmentName));
      }

      return environmentInfo;
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
