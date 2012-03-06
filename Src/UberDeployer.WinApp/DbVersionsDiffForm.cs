using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Domain;
using UberDeployer.Core.DataAccess.Dapper;

namespace UberDeployer.WinApp
{
  public partial class DbVersionsDiffForm : UberDeployerForm
  {
    private static readonly HashSet<string> _IgnoredDatabaseNames = new HashSet<string> { "MASTER", "MODEL", "MSDB", "TEMPDB", };
    
    private DbVersionsModel _currentDbVersionsModel;

    #region Constructor(s)

    public DbVersionsDiffForm()
    {
      InitializeComponent();
    }

    #endregion

    #region WinForms event handlers

    private void DbVersionsDiffForm_Load(object sender, EventArgs e)
    {
      dgv_environments.AutoGenerateColumns = false;

      LoadEnvironments();
    }

    private void dgv_environments_SelectionChanged(object sender, EventArgs e)
    {
      gb_diffResults.Enabled = (dgv_environments.SelectedRows.Count >= 2);
    }

    private void dgv_databasesInEnvironments_SelectionChanged(object sender, EventArgs e)
    {
      if (dgv_databasesInEnvironments.SelectedRows.Count == 0)
      {
        return;
      }

      if (_currentDbVersionsModel == null)
      {
        return;
      }

      object selectedRowHeaderValue = dgv_databasesInEnvironments.SelectedRows[0].HeaderCell.Value;

      if (selectedRowHeaderValue == null)
      {
        return;
      }

      string databaseName = selectedRowHeaderValue.ToString();

      DoDrillDown(databaseName);
    }

    private void dgv_databasesInEnvironments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      FormatGridViewCell(e);
    }

    private void dgv_databasesVersions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      FormatGridViewCell(e);
    }

    private void btn_diff_Click(object sender, EventArgs e)
    {
      DoDiff();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    #endregion

    #region Private helper methods

    private static List<string> ObtainDbVersions(SqlConnection dbConnection, string databaseName, string versionTableName, string versionColumnName)
    {
      IEnumerable<dynamic> dbVersions =
        dbConnection.Query(
          string.Format(
            "use [{0}]" + "\r\n" +
            "select [{1}] from [{2}]",
            databaseName,
            versionColumnName,
            versionTableName));

      return
        dbVersions
          .Select(dbVersion =>
                    {
                      object value = ((IDictionary<string, object>)dbVersion)[versionColumnName];

                      return value != null ? (string)value : null;
                    })
          .ToList();
    }

    private static bool TableContainsColumn(SqlConnection dbConnection, string databaseName, string tableName, string columnName)
    {
      IEnumerable<dynamic> result =
        dbConnection.Query(
          string.Format(
            "use [{0}]" + "\r\n" +
            "select * from sys.columns c" + "\r\n" +
            "join sys.tables t on t.[object_id] = c.[object_id]" + "\r\n" +
            "where t.name = '{1}' and c.name = '{2}'",
            databaseName,
            tableName,
            columnName));

      return result.Count() > 0;
    }

    private static void FormatGridViewCell(DataGridViewCellFormattingEventArgs e)
    {
      string valueString = e.Value as string;

      if (valueString == "X")
      {
        e.CellStyle.BackColor = Color.Red;
        e.CellStyle.SelectionBackColor = Color.DarkRed;
      }
      else if (valueString == "V")
      {
        e.CellStyle.BackColor = Color.Green;
        e.CellStyle.SelectionBackColor = Color.DarkGreen;
      }
    }

    private void LoadEnvironments()
    {
      GuiUtils.BeginInvoke(this, () => { dgv_environments.DataSource = null; });

      ThreadPool.QueueUserWorkItem(
        state =>
        {
          try
          {
            ToggleIndeterminateProgress(true, pic_indeterminateProgress);

            IEnvironmentInfoRepository environmentInfoRepository = ObjectFactory.Instance.CreateEnvironmentInfoRepository();

            List<EnvironmentInfo> allEnvironmentInfos =
              environmentInfoRepository.GetAll()
                .ToList();

            allEnvironmentInfos.Sort((ei1, ei2) => string.Compare(ei1.Name, ei2.Name));

            GuiUtils.BeginInvoke(
              this,
              () =>
                {
                  dgv_environments.DataSource =
                    allEnvironmentInfos
                      .Select(ei => new EnvironmentInfoRow(ei))
                      .ToList();

                  SelectAllEnvironments();
                });
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

    private void SelectAllEnvironments()
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            for (int i = 0; i < dgv_environments.Rows.Count; i++)
            {
              dgv_environments.Rows[i].Selected = true;
            }
          });
    }

    private void DoDiff()
    {
      // TODO IMM HI: refucktor
      ThreadPool.QueueUserWorkItem(
        state =>
          {
            try
            {
              ToggleIndeterminateProgress(true, pic_indeterminateProgress);

              var selectedEnvironments = new List<EnvironmentInfo>();

              for (int i = 0; i < dgv_environments.SelectedRows.Count; i++)
              {
                DataGridViewRow dataGridViewRow = dgv_environments.SelectedRows[i];
                var dataBoundItem = (EnvironmentInfoRow)dataGridViewRow.DataBoundItem;
                EnvironmentInfo environmentInfo = dataBoundItem.EnvironmentInfo;

                selectedEnvironments.Add(environmentInfo);
              }

              if (selectedEnvironments.Count < 2)
              {
                throw new InternalException("At least 2 environments should've been selected.");
              }

              selectedEnvironments.Sort((ei1, ei2) => string.Compare(ei1.Name, ei2.Name));

              var dbVersionsModel = new DbVersionsModel();

              foreach (EnvironmentInfo selectedEnvironment in selectedEnvironments)
              {
                string databaseServerMachineName = selectedEnvironment.DatabaseServerMachineName;
                string connectionString = string.Format("Server={0};Integrated Security=SSPI", databaseServerMachineName);

                using (var dbConnection = new SqlConnection(connectionString))
                {
                  dbConnection.Open();

                  IEnumerable<dynamic> databases =
                    dbConnection.Query("select * from sys.databases");

                  foreach (dynamic database in databases)
                  {
                    string databaseName = database.name.ToUpper();

                    if (_IgnoredDatabaseNames.Contains(databaseName))
                    {
                      continue;
                    }

                    IEnumerable<dynamic> tables;

                    try
                    {
                      tables = dbConnection.Query(
                        string.Format(
                          "use [{0}]" + "\r\n" +
                          "select * from sys.tables",
                          databaseName));
                    }
                    catch (SqlException)
                    {
                      tables = new List<dynamic>();
                    }

                    IEnumerable<string> tableNames = tables.Select(t => ((string)t.name).ToUpper());
                    List<string> dbVersions;

                    // TODO IMM HI: parameterize
                    if (tableNames.Contains("VERSIONHISTORY") && TableContainsColumn(dbConnection, databaseName, "VERSIONHISTORY", "DBLabel"))
                    {
                      dbVersions = ObtainDbVersions(dbConnection, databaseName, "VERSIONHISTORY", "DBLabel");
                    }
                    else if (tableNames.Contains("VERSION") && TableContainsColumn(dbConnection, databaseName, "VERSION", "dbVersion"))
                    {
                      dbVersions = ObtainDbVersions(dbConnection, databaseName, "VERSION", "dbVersion");
                    }
                    else
                    {
                      dbVersions = new List<string>();
                    }

                    dbVersionsModel.AddDatabase(selectedEnvironment.Name, databaseName, dbVersions);
                  }
                }
              }

              var databasesDataTable = new DataTable("Databases");

              foreach (EnvironmentInfo selectedEnvironment in selectedEnvironments)
              {
                databasesDataTable.Columns.Add(selectedEnvironment.Name);
              }

              bool differencesOnly = cb_differencesOnly.Checked;
              List<string> allSortedDatabaseNames = dbVersionsModel.GetAllSortedDatabaseNames().ToList();
              var addedDatabaseNames = new List<string>();

              foreach (string databaseName in allSortedDatabaseNames)
              {
                if (differencesOnly)
                {
                  bool areDatabasesConsistent =
                    dbVersionsModel.AreDatabasesConsistentAcrossEnvironments(databaseName);

                  if (areDatabasesConsistent)
                  {
                    continue;
                  }
                }

                string[] values = new string[selectedEnvironments.Count];
                int i = 0;

                foreach (EnvironmentInfo selectedEnvironment in selectedEnvironments)
                {
                  values[i++] = dbVersionsModel.IsDatabasePresentInEnvironment(selectedEnvironment.Name, databaseName) ? "V" : "X";
                }

                addedDatabaseNames.Add(databaseName);
                databasesDataTable.Rows.Add(values);
              }

              GuiUtils.BeginInvoke(
                this,
                () =>
                  {
                    _currentDbVersionsModel = dbVersionsModel;

                    dgv_databasesInEnvironments.DataSource = databasesDataTable;

                    int i = 0;

                    foreach (string databaseName in addedDatabaseNames)
                    {
                      dgv_databasesInEnvironments.Rows[i++].HeaderCell.Value = databaseName;
                    }

                    dgv_databasesInEnvironments.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

                    if (dgv_databasesInEnvironments.Rows.Count > 0)
                    {
                      // trigger SelectionChanged event
                      dgv_databasesInEnvironments.Rows[0].Selected = false;
                      dgv_databasesInEnvironments.Rows[0].Selected = true;
                    }
                  });
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

    private void DoDrillDown(string databaseName)
    {
      List<string> allSortedEnvironmentNames =
        new List<string>(_currentDbVersionsModel.GetAllSortedEnvironmentNames());

      var dbVersionsDataTable = new DataTable("DbVersions");

      foreach (string environmentName in allSortedEnvironmentNames)
      {
        dbVersionsDataTable.Columns.Add(environmentName);
      }

      IEnumerable<string> allSortedDbVersions = _currentDbVersionsModel.GetAllSortedDbVersions(databaseName);
      var addedDbVersions = new List<string>();

      foreach (string dbVersion in allSortedDbVersions)
      {
        string[] values = new string[allSortedEnvironmentNames.Count()];
        int i = 0;

        foreach (string environmentName in allSortedEnvironmentNames)
        {
          values[i++] = _currentDbVersionsModel.IsDbVersionPresent(environmentName, databaseName, dbVersion) ? "V" : "X";
        }

        addedDbVersions.Add(dbVersion);
        dbVersionsDataTable.Rows.Add(values);
      }

      dgv_databasesVersions.DataSource = dbVersionsDataTable;

      int j = 0;

      foreach (string dbVersion in addedDbVersions)
      {
        dgv_databasesVersions.Rows[j++].HeaderCell.Value = dbVersion;
      }

      dgv_databasesVersions.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

      if (dgv_databasesVersions.Rows.Count > 0)
      {
        dgv_databasesVersions.Rows[0].Selected = false;
      }
    }

    #endregion

    #region Nested types

    private class EnvironmentInfoRow
    {
      public EnvironmentInfoRow(EnvironmentInfo environmentInfo)
      {
        if (environmentInfo == null)
        {
          throw new ArgumentNullException("environmentInfo");
        }

        EnvironmentInfo = environmentInfo;
      }

      public EnvironmentInfo EnvironmentInfo { get; private set; }

      // ReSharper disable UnusedMember.Local

      public string Name
      {
        get { return EnvironmentInfo.Name; }
      }

      public string Server
      {
        get { return EnvironmentInfo.DatabaseServerMachineName; }
      }

      // ReSharper restore UnusedMember.Local
    }

    #endregion
  }
}
