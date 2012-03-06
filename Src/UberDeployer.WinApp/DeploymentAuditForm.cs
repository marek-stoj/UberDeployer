using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.WinApp.ViewModels;

namespace UberDeployer.WinApp
{
  public partial class DeploymentAuditForm : UberDeployerForm
  {
    private const int _MaxDeploymentRequestsCount = 100;

    #region Constructor(s)

    public DeploymentAuditForm()
    {
      InitializeComponent();
    }

    #endregion

    #region WinForms event handlers

    private void DeploymentAuditForm_Load(object sender, EventArgs e)
    {
      dgv_deploymentRequests.AutoGenerateColumns = false;
      grp_deploymentRequests.Text += string.Format(" (last {0})", _MaxDeploymentRequestsCount);
    }

    private void DeploymentAuditForm_Shown(object sender, EventArgs e)
    {
      LoadDeploymentRequests();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_reloadDeploymentRequests_Click(object sender, EventArgs e)
    {
      LoadDeploymentRequests();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    #endregion

    #region Private helper methods

    private void LoadDeploymentRequests()
    {
      GuiUtils.BeginInvoke(this, () => dgv_deploymentRequests.DataSource = null);

      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              IDeploymentRequestRepository deploymentRequestRepository = ObjectFactory.Instance.CreateDeploymentRequestRepository();

              IEnumerable<DeploymentRequestInListViewModel> deploymentRequests =
                deploymentRequestRepository.GetDeploymentRequests(0, _MaxDeploymentRequestsCount)
                  .Select(dr => new DeploymentRequestInListViewModel(dr))
                  .ToList();

              GuiUtils.BeginInvoke(this, () => { dgv_deploymentRequests.DataSource = deploymentRequests; });
            }
            catch (Exception exc)
            {
              HandleThreadException(exc);
            }
            finally
            {
              ToggleIndeterminateProgress(false, pic_indeterminateProgress);
            }
          });
    }

    #endregion
  }
}
