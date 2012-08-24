using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Agent.Proxy.Dto.TeamCity;
using UberDeployer.Agent.Proxy.Faults;
using UberDeployer.WinApp.Utils;
using UberDeployer.WinApp.ViewModels;

namespace UberDeployer.WinApp.Forms
{
  // TODO IMM HI: multiple loads
  public partial class MainForm : UberDeployerForm
  {
    private const int _MaxProjectConfigurationBuildsCount = 10;

    private bool _suppressProjectConfigurationsLoading;

    private int _projectsRequestsCounter;
    private int _projectConfigurationsRequestsCounter;
    private int _projectConfigurationBuildsRequestsCounter;

    private readonly object _projectsRequestsMutex = new object();
    private readonly object _projectConfigurationsRequestsMutex = new object();
    private readonly object _projectConfigurationBuildsRequestsMutex = new object();
    
    private readonly IAgentService _agentServiceClient;

    #region Constructor(s)

    public MainForm()
    {
      InitializeComponent();

      _agentServiceClient = new AgentServiceClient();
    }

    #endregion

    #region WinForms event handlers

    private void MainForm_Load(object sender, EventArgs e)
    {
      dgv_projectInfos.AutoGenerateColumns = false;
      dgv_projectConfigurations.AutoGenerateColumns = false;
      dgv_projectConfigurationBuilds.AutoGenerateColumns = false;

      grp_projectConfigurationBuilds.Text += string.Format(" (last {0})", _MaxProjectConfigurationBuildsCount);

      cb_messageTypeThreshold.Items.Clear();
      cb_messageTypeThreshold.Items.AddRange(Enum.GetValues(typeof(MessageType)).Cast<object>().ToArray());
      cb_messageTypeThreshold.SelectedItem = MessageType.Info;
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
      LoadEnvironments();
      LoadProjects();
    }

    private void dgv_projectInfos_SelectionChanged(object sender, EventArgs e)
    {
      ClearProjectConfigurationsList();

      if (dgv_projectInfos.SelectedRows.Count == 0)
      {
        ToggleProjectContextButtonsEnabled(false);
        return;
      }

      if (_suppressProjectConfigurationsLoading)
      {
        return;
      }

      ToggleProjectContextButtonsEnabled(true);

      ProjectInfo projectInfo = GetSelectedProjectInfo();

      btn_openWebApp.Enabled = (projectInfo is WebAppProjectInfo);

      LoadProjectConfigurations(projectInfo);
    }

    private void dgv_projectConfigurations_SelectionChanged(object sender, EventArgs e)
    {
      ClearProjectConfigurationBuildsList();

      if (dgv_projectConfigurations.SelectedRows.Count == 0)
      {
        ToggleProjectConfigurationContextButtonsEnabled(false);
        return;
      }

      ToggleProjectConfigurationContextButtonsEnabled(true);

      var projectConfigurationInListViewModel = (ProjectConfigurationInListViewModel)dgv_projectConfigurations.SelectedRows[0].DataBoundItem;
      string projectName = projectConfigurationInListViewModel.ProjectName;
      ProjectConfiguration projectConfiguration = projectConfigurationInListViewModel.ProjectConfiguration;

      LoadProjectConfigurationBuilds(projectName, projectConfiguration);
    }

    private void dgv_projectConfigurations_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgv_projectConfigurations.Rows.Count)
      {
        return;
      }

      if (e.ColumnIndex < 0 || e.ColumnIndex >= dgv_projectConfigurations.Columns.Count)
      {
        return;
      }

      if (dgv_projectConfigurations.Columns[e.ColumnIndex].Name != "ProjectConfigurationWebLinkColumn")
      {
        return;
      }

      OpenProjectConfigurationInBrowser(e.RowIndex);
    }

    private void dgv_projectConfigurations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgv_projectConfigurations.Rows.Count)
      {
        return;
      }

      object dataBoundItem = dgv_projectConfigurations.Rows[e.RowIndex].DataBoundItem;
      var projectConfigurationBuild = ((ProjectConfigurationInListViewModel)dataBoundItem).ProjectConfiguration;

      OpenUrlInBrowser(projectConfigurationBuild.WebUrl);
    }

    private void dgv_projectConfigurationBuilds_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgv_projectConfigurationBuilds.Rows.Count)
      {
        return;
      }

      if (e.ColumnIndex < 0 || e.ColumnIndex >= dgv_projectConfigurationBuilds.Columns.Count)
      {
        return;
      }

      if (dgv_projectConfigurationBuilds.Columns[e.ColumnIndex].Name != "ProjectConfigurationBuildWebLinkColumn")
      {
        return;
      }

      OpenProjectConfigurationBuildInBrowser(e.RowIndex);
    }

    private void dgv_projectConfigurationBuilds_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgv_projectConfigurationBuilds.Rows.Count)
      {
        return;
      }

      object dataBoundItem = dgv_projectConfigurationBuilds.Rows[e.RowIndex].DataBoundItem;
      var projectConfigurationBuild = ((ProjectConfigurationBuildInListViewModel)dataBoundItem).ProjectConfigurationBuild;

      OpenUrlInBrowser(projectConfigurationBuild.WebUrl);
    }

    private void deployBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      bool startSeparatorWasLogged = false;

      try
      {
        ToggleIndeterminateProgress(true, pic_indeterminateProgress);

        var projectDeploymentInfo = (ProjectDeploymentInfo)e.Argument;

        ProjectInfo projectInfo = projectDeploymentInfo.ProjectInfo;
        ProjectConfiguration projectConfiguration = projectDeploymentInfo.ProjectConfiguration;
        ProjectConfigurationBuild projectConfigurationBuild = projectDeploymentInfo.ProjectConfigurationBuild;

        LogMessage(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", MessageType.Info);
        startSeparatorWasLogged = true;

        // TODO IMM HI: xxx get log messages from service (WCF duplex?)
        _agentServiceClient
          .BeginDeploymentJob(
            projectInfo.Name,
            projectConfiguration.Name,
            projectConfigurationBuild.Id,
            projectDeploymentInfo.TargetEnvironmentName);
      }
      catch (Exception exc)
      {
        LogMessage("Error: " + exc.Message, MessageType.Error);
      }
      finally
      {
        if (startSeparatorWasLogged)
        {
          LogMessage("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<", MessageType.Info);
        }

        ToggleIndeterminateProgress(false, pic_indeterminateProgress);
      }
    }

    // TODO IMM HI: implement
    private void btn_describeDeployment_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Not implemented yet!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btn_deploy_Click(object sender, EventArgs e)
    {
      if (dgv_projectConfigurations.SelectedRows.Count == 0)
      {
        AppUtils.NotifyUserInvalidOperation("No project configuration is selected.");
        return;
      }

      if (dgv_projectConfigurationBuilds.SelectedRows.Count == 0)
      {
        AppUtils.NotifyUserInvalidOperation("No project configuration build is selected.");
        return;
      }

      if (cbx_targetEnvironment.SelectedItem == null)
      {
        AppUtils.NotifyUserInvalidOperation("No target environment is selected.");
        return;
      }

      ProjectInfo projectInfo = GetSelectedProjectInfo();
      ProjectConfiguration projectConfiguration = GetSelectedProjectConfiguration();
      ProjectConfigurationBuild projectConfigurationBuild = GetSelectedProjectConfigurationBuild();

      if (projectConfigurationBuild.Status != BuildStatus.Success)
      {
        AppUtils.NotifyUserInvalidOperation("Can't deploy a build which is not successful.");
        return;
      }

      string targetEnvironmentName = GetSelectedEnvironment().Name;

      Deploy(new ProjectDeploymentInfo(projectInfo, projectConfiguration, projectConfigurationBuild, targetEnvironmentName));
    }

    private void btn_clearLog_Click(object sender, EventArgs e)
    {
      txt_log.Clear();
    }

    private void btn_showProjectInfo_Click(object sender, EventArgs e)
    {
      if (dgv_projectInfos.SelectedRows.Count == 0)
      {
        AppUtils.NotifyUserInvalidOperation("No project is selected.");
        return;
      }

      ProjectInfo projectInfo = GetSelectedProjectInfo();

      if (projectInfo == null)
      {
        return;
      }

      var viewProjectInfoForm = new ViewProjectInfoForm();

      viewProjectInfoForm.LoadProjectInfo(projectInfo);
      viewProjectInfoForm.ShowDialog(this);
    }

    private void btn_openProjectTargetFolder_Click(object sender, EventArgs e)
    {
      if (dgv_projectInfos.SelectedRows.Count == 0)
      {
        AppUtils.NotifyUserInvalidOperation("No project is selected.");
        return;
      }

      if (cbx_targetEnvironment.SelectedItem == null)
      {
        AppUtils.NotifyUserInvalidOperation("No target environment is selected.");
        return;
      }

      ProjectInfo projectInfo = GetSelectedProjectInfo();
      EnvironmentInfo environmentInfo = GetSelectedEnvironment();

      OpenProjectTargetFolder(projectInfo, environmentInfo);
    }

    private void btn_openWebApp_Click(object sender, EventArgs e)
    {
      if (dgv_projectInfos.SelectedRows.Count == 0)
      {
        AppUtils.NotifyUserInvalidOperation("No project is selected.");
        return;
      }

      if (cbx_targetEnvironment.SelectedItem == null)
      {
        AppUtils.NotifyUserInvalidOperation("No target environment is selected.");
        return;
      }

      ProjectInfo projectInfo = GetSelectedProjectInfo();
      WebAppProjectInfo webAppProjectInfo = projectInfo as WebAppProjectInfo;

      if (webAppProjectInfo == null)
      {
        AppUtils.NotifyUserInvalidOperation("Selected project is not a web application.");
        return;
      }

      EnvironmentInfo environmentInfo = GetSelectedEnvironment();

      OpenWebApp(webAppProjectInfo, environmentInfo);
    }

    private void btn_showEnvironmentInfo_Click(object sender, EventArgs e)
    {
      if (cbx_targetEnvironment.SelectedItem == null)
      {
        AppUtils.NotifyUserInvalidOperation("No target environment is selected.");
        return;
      }

      EnvironmentInfo selectedEnvironmentInfo = GetSelectedEnvironment();

      var viewEnvironmentInfoForm = new ViewEnvironmentInfoForm();

      viewEnvironmentInfoForm.LoadEnvironmentInfo(selectedEnvironmentInfo);
      viewEnvironmentInfoForm.ShowDialog(this);
    }

    private void btn_showDependencies_Click(object sender, EventArgs e)
    {
      ProjectConfiguration selectedProjectConfiguration = GetSelectedProjectConfiguration();
      var projectDepsVisualizerMainForm = new ProjectDepsVisualizer.UI.MainForm();

      projectDepsVisualizerMainForm
        .SetProjectConfiguration(
          selectedProjectConfiguration.ProjectName,
          selectedProjectConfiguration.Name);

      projectDepsVisualizerMainForm.Show();
    }

    private void OpenWebApp(WebAppProjectInfo webAppProjectInfo, EnvironmentInfo environmentInfo)
    {
      List<string> targetUrls =
        _agentServiceClient.GetWebAppProjectTargetUrls(
          webAppProjectInfo.Name,
          environmentInfo.Name);

      if (targetUrls.Count == 1)
      {
        Process.Start(targetUrls[0]);
      }
      else
      {
        new OpenTargetUrlsForm(targetUrls)
          .ShowDialog();
      }
    }

    private void reloadProjectsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      LoadProjects();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void deploymentAuditToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var deploymentAuditForm = new DeploymentAuditForm();

      deploymentAuditForm.Show();
    }

    private void dbVersionsDiffToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var dbVersionsDiffForm = new DbVersionsDiffForm();

      dbVersionsDiffForm.Show();
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
      ProjectFilter projectFilter =
        !string.IsNullOrEmpty(txtFilter.Text)
          ? new ProjectFilter { Name = txtFilter.Text, }
          : ProjectFilter.Empty;

      List<ProjectInfo> projectInfos =
        _agentServiceClient.GetProjectInfos(projectFilter);

      List<ProjectInfoInListViewModel> projectInfoViewModels =
        projectInfos
          .Select(p => new ProjectInfoInListViewModel(p))
          .ToList();

      GuiUtils.BeginInvoke(this, () => dgv_projectInfos.DataSource = projectInfoViewModels);
    }

    private void txtFilterConfigs_TextChanged(object sender, EventArgs e)
    {
      ProjectInfo projectInfo = GetSelectedProjectInfo();

      ProjectConfigurationFilter projectConfigurationFilter =
        !string.IsNullOrEmpty(txtFilterConfigs.Text)
          ? new ProjectConfigurationFilter { Name = txtFilterConfigs.Text, }
          : ProjectConfigurationFilter.Empty;

      List<ProjectConfiguration> projectConfigurations =
        _agentServiceClient.GetProjectConfigurations(
          projectInfo.Name,
          projectConfigurationFilter);

      List<ProjectConfigurationInListViewModel> projectConfigurationViewModels =
        projectConfigurations
          .Select(pc => new ProjectConfigurationInListViewModel(projectInfo.Name, pc))
          .ToList();

      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurations.DataSource = projectConfigurationViewModels);
    }

    private void txtFilterBuilds_TextChanged(object sender, EventArgs e)
    {
      ProjectInfo projectInfo = GetSelectedProjectInfo();
      ProjectConfiguration projectConfiguration = GetSelectedProjectConfiguration();

      var projectConfigurationBuildFilter =
        !string.IsNullOrEmpty(txtFilterBuilds.Text)
          ? new ProjectConfigurationBuildFilter { Number = txtFilterBuilds.Text, }
          : ProjectConfigurationBuildFilter.Empty;

      List<ProjectConfigurationBuild> projectConfigurationBuilds =
        _agentServiceClient.GetProjectConfigurationBuilds(
          projectInfo.Name,
          projectConfiguration.Name,
          _MaxProjectConfigurationBuildsCount,
          projectConfigurationBuildFilter);

      List<ProjectConfigurationBuildInListViewModel> projectConfigurationBuildViewModels =
        projectConfigurationBuilds
          .Select(pcb => new ProjectConfigurationBuildInListViewModel { ProjectConfigurationBuild = pcb })
          .ToList();

      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurationBuilds.DataSource = projectConfigurationBuildViewModels);
    }

    private void dependenciesVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var projectDepsVisualizerMainForm = new ProjectDepsVisualizer.UI.MainForm();

      projectDepsVisualizerMainForm.Show();
    }

    #endregion

    #region Private methods

    private void OpenProjectTargetFolder(ProjectInfo projectInfo, EnvironmentInfo environmentInfo)
    {
      List<string> projectTargetFolders =
        _agentServiceClient.GetProjectTargetFolders(
          projectInfo.Name,
          environmentInfo.Name);

      if (projectTargetFolders.Count == 1)
      {
        SystemUtils.OpenFolder(projectTargetFolders[0]);
      }
      else
      {
        new OpenTargetFoldersForm(projectTargetFolders)
          .ShowDialog();
      }
    }

    private void LoadProjects()
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            dgv_projectInfos.DataSource = null;

            txtFilter.Text = "";
            txtFilterConfigs.Text = "";
            txtFilterBuilds.Text = "";
          });

      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              int requestNumber;

              lock (_projectsRequestsMutex)
              {
                _projectsRequestsCounter++;
                requestNumber = _projectsRequestsCounter;
              }

              LogMessage("Loading projects...", MessageType.Trace);
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              List<ProjectInfoInListViewModel> allProjects =
                _agentServiceClient.GetProjectInfos(ProjectFilter.Empty)
                  .Select(p => new ProjectInfoInListViewModel(p))
                  .ToList();

              lock (_projectsRequestsMutex)
              {
                if (requestNumber != _projectsRequestsCounter)
                {
                  // don't update UI because there already was a new request
                  return;
                }

                GuiUtils.BeginInvoke(
                  this,
                  () =>
                  {
                    try
                    {
                      _suppressProjectConfigurationsLoading = true;
                      dgv_projectInfos.DataSource = allProjects;
                    }
                    finally
                    {
                      _suppressProjectConfigurationsLoading = false;
                    }

                    dgv_projectInfos.ClearSelection();
                  });
              }
            }
            catch (Exception exc)
            {
              HandleThreadException(exc);
            }
            finally
            {
              ToggleIndeterminateProgress(false, pic_indeterminateProgress);
              LogMessage("Done loading projects.", MessageType.Trace);
            }
          });
    }

    private void ClearProjectConfigurationsList()
    {
      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurations.DataSource = new List<ProjectConfigurationInListViewModel>());
    }

    private void LoadProjectConfigurations(ProjectInfo projectInfo)
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            dgv_projectConfigurations.DataSource = null;

            txtFilterConfigs.Text = "";
            txtFilterBuilds.Text = "";
          });

      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              int requestNumber;

              lock (_projectConfigurationsRequestsMutex)
              {
                _projectConfigurationsRequestsCounter++;
                requestNumber = _projectConfigurationsRequestsCounter;
              }

              LogMessage(string.Format("Loading project configurations for project: '{0}'...", projectInfo.Name), MessageType.Trace);
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              List<ProjectConfigurationInListViewModel> projectConfigurations;

              try
              {
                projectConfigurations =
                  _agentServiceClient.GetProjectConfigurations(projectInfo.Name, ProjectConfigurationFilter.Empty)
                    .Select(pc => new ProjectConfigurationInListViewModel(projectInfo.Name, pc))
                    .ToList();
              }
              catch (FaultException<ProjectNotFoundFault>)
              {
                LogMessage(string.Format("Project with artifacts repository name '{0}' couldn't be found.", projectInfo.ArtifactsRepositoryName), MessageType.Trace);

                projectConfigurations = new List<ProjectConfigurationInListViewModel>();
              }

              lock (_projectConfigurationsRequestsMutex)
              {
                if (requestNumber != _projectConfigurationsRequestsCounter)
                {
                  // don't update UI because there already was a new request
                  return;
                }

                GuiUtils.BeginInvoke(this, () => dgv_projectConfigurations.DataSource = projectConfigurations);
              }
            }
            catch (Exception exc)
            {
              HandleThreadException(exc);
            }
            finally
            {
              ToggleIndeterminateProgress(false, pic_indeterminateProgress);
              LogMessage(string.Format("Done loading project configurations for project: '{0}'.", projectInfo.Name), MessageType.Trace);
            }
          });
    }

    private void ClearProjectConfigurationBuildsList()
    {
      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurationBuilds.DataSource = new List<ProjectConfigurationBuildInListViewModel>());
    }

    private void LoadProjectConfigurationBuilds(string projectName, ProjectConfiguration projectConfiguration)
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            dgv_projectConfigurationBuilds.DataSource = null;

            txtFilterBuilds.Text = "";
          });

      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              int requestNumber;

              lock (_projectConfigurationBuildsRequestsMutex)
              {
                _projectConfigurationBuildsRequestsCounter++;
                requestNumber = _projectConfigurationBuildsRequestsCounter;
              }

              LogMessage(string.Format("Loading project configuration builds for project configuration: '{0} ({1})'...", projectName, projectConfiguration.Name), MessageType.Trace);
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              List<ProjectConfigurationBuildInListViewModel> projectConfigurationBuilds =
                _agentServiceClient.GetProjectConfigurationBuilds(projectName, projectConfiguration.Name, _MaxProjectConfigurationBuildsCount, ProjectConfigurationBuildFilter.Empty)
                  .Select(pcb => new ProjectConfigurationBuildInListViewModel { ProjectConfigurationBuild = pcb })
                  .ToList();

              lock (_projectConfigurationBuildsRequestsMutex)
              {
                if (requestNumber != _projectConfigurationBuildsRequestsCounter)
                {
                  // don't update UI because there already was a new request
                  return;
                }

                GuiUtils.BeginInvoke(this, () => dgv_projectConfigurationBuilds.DataSource = projectConfigurationBuilds);
              }
            }
            catch (Exception exc)
            {
              HandleThreadException(exc);
            }
            finally
            {
              ToggleIndeterminateProgress(false, pic_indeterminateProgress);
              LogMessage(string.Format("Done loading project configuration builds for project configuration: '{0} ({1})'.", projectConfiguration.ProjectName, projectConfiguration.Name), MessageType.Trace);
            }
          });
    }

    private void Deploy(ProjectDeploymentInfo projectDeploymentInfo)
    {
      var deployBackgroundWorker = new BackgroundWorker();

      deployBackgroundWorker.DoWork += deployBackgroundWorker_DoWork;
      deployBackgroundWorker.RunWorkerAsync(projectDeploymentInfo);
    }

    private void LogMessage(string message, MessageType messageType = MessageType.Info)
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            if (messageType < MessageTypeThreshold)
            {
              return;
            }

            txt_log.SelectionStart = txt_log.Text.Length;
            txt_log.SelectionLength = 0;

            switch (messageType)
            {
              case MessageType.Trace:
                txt_log.SelectionColor = Color.DimGray;
                break;
              case MessageType.Info:
                txt_log.SelectionColor = Color.Blue;
                break;
              case MessageType.Warning:
                txt_log.SelectionColor = Color.FromArgb(191, 79, 0);
                break;
              case MessageType.Error:
                txt_log.SelectionColor = Color.DarkRed;
                break;
            }

            txt_log.AppendText(message);
            txt_log.AppendText(Environment.NewLine);

            txt_log.ScrollToCaret();
          });
    }

    private void LoadEnvironments()
    {
      GuiUtils.BeginInvoke(this, () => { cbx_targetEnvironment.DataSource = null; });

      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              LogMessage("Loading environments...", MessageType.Trace);
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              List<EnvironmentInfoInComboBoxViewModel> allEnvironmentInfos =
                _agentServiceClient.GetEnvironmentInfos()
                  .Select(ei => new EnvironmentInfoInComboBoxViewModel { EnvironmentInfo = ei })
                  .ToList();

              GuiUtils.BeginInvoke(this, () => { cbx_targetEnvironment.DataSource = allEnvironmentInfos; });
            }
            catch (Exception exc)
            {
              HandleThreadException(exc);
            }
            finally
            {
              ToggleIndeterminateProgress(false, pic_indeterminateProgress);
              LogMessage("Done loading environments.", MessageType.Trace);
            }
          });
    }

    private void OpenProjectConfigurationInBrowser(int rowIndex)
    {
      object dataBoundItem = dgv_projectConfigurations.Rows[rowIndex].DataBoundItem;
      var projectConfiguration = ((ProjectConfigurationInListViewModel)dataBoundItem).ProjectConfiguration;

      OpenUrlInBrowser(projectConfiguration.WebUrl);
    }

    private void OpenProjectConfigurationBuildInBrowser(int rowIndex)
    {
      object dataBoundItem = dgv_projectConfigurationBuilds.Rows[rowIndex].DataBoundItem;
      var projectConfigurationBuild = ((ProjectConfigurationBuildInListViewModel)dataBoundItem).ProjectConfigurationBuild;

      OpenUrlInBrowser(projectConfigurationBuild.WebUrl);
    }

    private void OpenUrlInBrowser(string url)
    {
      Process.Start(url);
    }

    private ProjectInfo GetSelectedProjectInfo()
    {
      if (dgv_projectInfos.SelectedRows.Count == 0)
      {
        throw new InvalidOperationException("No project is selected.");
      }

      return ((ProjectInfoInListViewModel)dgv_projectInfos.SelectedRows[0].DataBoundItem).ProjectInfo;
    }

    private ProjectConfiguration GetSelectedProjectConfiguration()
    {
      if (dgv_projectConfigurations.SelectedRows.Count == 0)
      {
        throw new InvalidOperationException("No project confguration is selected.");
      }

      return ((ProjectConfigurationInListViewModel)dgv_projectConfigurations.SelectedRows[0].DataBoundItem).ProjectConfiguration;
    }

    private ProjectConfigurationBuild GetSelectedProjectConfigurationBuild()
    {
      if (dgv_projectConfigurationBuilds.SelectedRows.Count == 0)
      {
        throw new InvalidOperationException("No project confguration build is selected.");
      }

      return ((ProjectConfigurationBuildInListViewModel)dgv_projectConfigurationBuilds.SelectedRows[0].DataBoundItem).ProjectConfigurationBuild;
    }

    private void ToggleProjectContextButtonsEnabled(bool enabled)
    {
      btn_showProjectInfo.Enabled = enabled;
      btn_openProjectTargetFolder.Enabled = enabled;
      btn_openWebApp.Enabled = enabled;
    }

    private void ToggleProjectConfigurationContextButtonsEnabled(bool enabled)
    {
      btn_showDependencies.Enabled = enabled;
    }

    private EnvironmentInfo GetSelectedEnvironment()
    {
      if (cbx_targetEnvironment.SelectedItem == null)
      {
        throw new InvalidOperationException("No target environment is selected.");
      }

      return ((EnvironmentInfoInComboBoxViewModel)cbx_targetEnvironment.SelectedItem).EnvironmentInfo;
    }

    #endregion

    #region Private properties

    private MessageType MessageTypeThreshold
    {
      get { return (MessageType)cb_messageTypeThreshold.SelectedItem; }
    }

    #endregion
  }
}
