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
    protected readonly string _targetEnvironmentName;

    private readonly List<DeploymentTaskBase> _subTasks;
    
    private string _tempDirPath;

    #region Constructor(s)

    protected DeploymentTask(IEnvironmentInfoRepository environmentInfoRepository, string targetEnvironmentName)
    {
      if (environmentInfoRepository == null)
      {
        throw new ArgumentNullException("environmentInfoRepository");
      }

      if (string.IsNullOrEmpty(targetEnvironmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetEnvironmentName");
      }

      _environmentInfoRepository = environmentInfoRepository;
      _targetEnvironmentName = targetEnvironmentName;

      _subTasks = new List<DeploymentTaskBase>();
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      foreach (DeploymentTaskBase subTask in _subTasks)
      {
        subTask.Prepare();
        subTask.Execute();
      }

      PostDiagnosticMessage(string.Format("Finished '{0}' (\"{1}\").", GetType().Name, Description));
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
      EnvironmentInfo environmentInfo = _environmentInfoRepository.GetByName(_targetEnvironmentName);

      if (environmentInfo == null)
      {
        throw new DeploymentTaskException(string.Format("Environment named '{0}' doesn't exist.", _targetEnvironmentName));
      }

      return environmentInfo;
    }

    protected void CreateTemporaryDirectory()
    {
      string tempDirName = Guid.NewGuid().ToString("N");
      
      _tempDirPath = Path.Combine(Path.GetTempPath(), tempDirName);

      Directory.CreateDirectory(_tempDirPath);
    }

    protected void DeleteTemporaryDirectory()
    {
      if (!string.IsNullOrEmpty(_tempDirPath) && Directory.Exists(_tempDirPath))
      {
        Directory.Delete(_tempDirPath, true);
      }
    }

    protected string TempDirPath
    {
      get { return _tempDirPath; }
    }

    #endregion

    #region Properties

    // TODO IMM HI: refactor?
    public abstract string ProjectName { get; }

    public abstract string ProjectConfigurationName { get; }

    public string TargetEnvironmentName
    {
      get { return _targetEnvironmentName; }
    }

    #endregion
  }
}
