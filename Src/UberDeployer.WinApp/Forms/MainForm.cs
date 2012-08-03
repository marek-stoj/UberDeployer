using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;
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

    #region Constructor(s)

    public MainForm()
    {
      InitializeComponent();
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

      LoadProjectConfigurationBuilds(((ProjectConfigurationInListViewModel)dgv_projectConfigurations.SelectedRows[0].DataBoundItem).ProjectConfiguration);
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

        DeploymentTask deploymentTask =
          projectInfo.CreateDeploymentTask(
            ObjectFactory.Instance,
            projectConfiguration.Name,
            projectConfigurationBuild.Id,
            projectDeploymentInfo.TargetEnvironmentName);

        deploymentTask.DiagnosticMessagePosted +=
          (eventSender, args) => LogMessage(args.Message, args.MessageType);

        LogMessage(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", MessageType.Info);
        startSeparatorWasLogged = true;

        IDeploymentPipeline deploymentPipeline =
          ObjectFactory.Instance.CreateDeploymentPipeline();

        deploymentPipeline.StartDeployment(deploymentTask);
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

      if (targetEnvironmentName == EnforceTargetEnvironmentConstraintsModule.ProductionEnvironmentName
       && projectConfiguration.Name != EnforceTargetEnvironmentConstraintsModule.ProductionProjectConfigurationName)
      {
        AppUtils.NotifyUserInvalidOperation(string.Format("Can't deploy project ('{0}') with non-production configuration ('{1}') to the production environment!", projectConfiguration.ProjectName, projectConfiguration.Name));
        return;
      }

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
        webAppProjectInfo.GetTargetUrls(environmentInfo)
          .ToList();

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

    private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenConfigurationForm();
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
      IProjectInfoRepository projectInfoRepository = ObjectFactory.Instance.CreateProjectInfoRepository();

      IEnumerable<ProjectInfoInListViewModel> filteredProjects =
        projectInfoRepository.GetAll()
          .Select(p => new ProjectInfoInListViewModel(p))
          .Where(ob => ob.Name.ToLower().Contains(txtFilter.Text.ToLower()) || ob.Type.ToLower().Contains(txtFilter.Text.ToLower()))
          .ToList();

      dgv_projectInfos.DataSource = filteredProjects;
    }

    private void txtFilterConfigs_TextChanged(object sender, EventArgs e)
    {
      ProjectInfo projectInfo = GetSelectedProjectInfo();
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      
      Project project = teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName);
      ProjectDetails projectDetails = teamCityClient.GetProjectDetails(project);

      List<ProjectConfigurationInListViewModel> projectConfigurations =
        (projectDetails.ConfigurationsList != null && projectDetails.ConfigurationsList.Configurations != null)
          ? projectDetails.ConfigurationsList.Configurations
              .Select(pc => new ProjectConfigurationInListViewModel(pc))
              .Where(ob => ob.Name.ToLower().Contains(txtFilterConfigs.Text.ToLower()))
              .ToList()
          : new List<ProjectConfigurationInListViewModel>();

      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurations.DataSource = projectConfigurations);
    }

    private void txtFilterBuilds_TextChanged(object sender, EventArgs e)
    {
      ProjectConfiguration projectConfiguration = GetSelectedProjectConfiguration();
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      
      ProjectConfigurationDetails projectConfigurationDetails =
        teamCityClient.GetProjectConfigurationDetails(projectConfiguration);
      
      ProjectConfigurationBuildsList projectConfigurationBuildsList =
        teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, 0, _MaxProjectConfigurationBuildsCount);

      List<ProjectConfigurationBuildInListViewModel> projectConfigurationBuilds =
        (projectConfigurationBuildsList.Builds != null)
          ? projectConfigurationBuildsList.Builds
              .Select(pcb => new ProjectConfigurationBuildInListViewModel { ProjectConfigurationBuild = pcb })
              .Where(ob => ob.Id.ToLower().Contains(txtFilterBuilds.Text.ToLower()) ||
                           ob.Number.ToLower().Contains(txtFilterBuilds.Text.ToLower()) ||
                           ob.Status.ToLower().Contains(txtFilterBuilds.Text.ToLower()))
              .ToList()
          : new List<ProjectConfigurationBuildInListViewModel>();

      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurationBuilds.DataSource = projectConfigurationBuilds);
    }

    private void dependenciesVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var projectDepsVisualizerMainForm = new ProjectDepsVisualizer.UI.MainForm();

      projectDepsVisualizerMainForm.Show();
    }

    #endregion

    #region Private methods

    private static void OpenProjectTargetFolder(ProjectInfo projectInfo, EnvironmentInfo environmentInfo)
    {
      List<string> projectTargetFolders =
        projectInfo.GetTargetFolders(environmentInfo)
          .ToList();

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
      GuiUtils.BeginInvoke(this, () => { dgv_projectInfos.DataSource = null; });

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

              IProjectInfoRepository projectInfoRepository = ObjectFactory.Instance.CreateProjectInfoRepository();

              IEnumerable<ProjectInfoInListViewModel> allProjects =
                projectInfoRepository.GetAll()
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
      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurations.DataSource = null);

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

              ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
              Project project = teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName);
              List<ProjectConfigurationInListViewModel> projectConfigurations;

              if (project != null)
              {
                ProjectDetails projectDetails = teamCityClient.GetProjectDetails(project);

                projectConfigurations =
                  (projectDetails.ConfigurationsList != null && projectDetails.ConfigurationsList.Configurations != null)
                    ? projectDetails.ConfigurationsList.Configurations
                        .Select(pc => new ProjectConfigurationInListViewModel(pc))
                        .ToList()
                    : new List<ProjectConfigurationInListViewModel>();
              }
              else
              {
                LogMessage(string.Format("Project named '{0}' couldn't be found in TeamCity.", projectInfo.ArtifactsRepositoryName), MessageType.Trace);

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

    private void LoadProjectConfigurationBuilds(ProjectConfiguration projectConfiguration)
    {
      GuiUtils.BeginInvoke(this, () => dgv_projectConfigurationBuilds.DataSource = null);

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

              LogMessage(string.Format("Loading project configuration builds for project configuration: '{0} ({1})'...", projectConfiguration.ProjectName, projectConfiguration.Name), MessageType.Trace);
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();

              ProjectConfigurationDetails projectConfigurationDetails =
                teamCityClient.GetProjectConfigurationDetails(projectConfiguration);

              ProjectConfigurationBuildsList projectConfigurationBuildsList =
                teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, 0, _MaxProjectConfigurationBuildsCount);

              List<ProjectConfigurationBuildInListViewModel> projectConfigurationBuilds =
                projectConfigurationBuildsList.Builds != null
                  ? projectConfigurationBuildsList.Builds
                      .Select(pcb => new ProjectConfigurationBuildInListViewModel { ProjectConfigurationBuild = pcb })
                      .ToList()
                  : new List<ProjectConfigurationBuildInListViewModel>();

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
      if (messageType < MessageTypeThreshold)
      {
        return;
      }

      GuiUtils.BeginInvoke(
        this,
        () =>
          {
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

              IEnvironmentInfoRepository environmentInfoRepository = ObjectFactory.Instance.CreateEnvironmentInfoRepository();

              var allEnvironmentInfos =
                environmentInfoRepository.GetAll()
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

    private void OpenConfigurationForm()
    {
      new ConfigurationForm()
        .ShowDialog();
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
