using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace UberDeployer.Core.Management.Db
{
  public class MsSqlSqlCmdScriptRunner : IDbScriptRunner
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static string SqlCmdExe = "sqlcmd.exe";

    private readonly string _databaseServer;
    private readonly string _databaseName;
    private string _tempDirPath;
    private string _tmpScriptPath;

    public MsSqlSqlCmdScriptRunner(string databaseServer, string databaseName)
    {
      if (string.IsNullOrEmpty(databaseServer))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseServer");
      }

      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      _databaseServer = databaseServer;
      _databaseName = databaseName;
    }

    public void Execute(string scriptToExecute)
    {
      if (scriptToExecute == null)
      {
        throw new ArgumentNullException("scriptToExecute");
      }

      _tmpScriptPath = Path.Combine(GetTempDirPath(), Guid.NewGuid().ToString("N") + ".sql");
      File.WriteAllText(_tmpScriptPath, scriptToExecute, new UTF8Encoding(true));

      try
      {
        Execute(
          SqlCmdExe,
          string.Format(
            "-S \"{0}\" -E -i \"{2}\" -V15 -b -v DatabaseName=\"{1}\"", _databaseServer, _databaseName, _tmpScriptPath));

        _log.Debug("Applying script to database ended successfully.");
      }
      catch (Exception exception)
      {
        _log.Error(exception);
        throw;
      }
      finally
      {
        File.Delete(_tmpScriptPath);
      }
    }

    private static void Execute(string fileToExecute, string arguments)
    {
      ProcessStartInfo processStartInfo = new ProcessStartInfo();
      processStartInfo.FileName = fileToExecute;
      processStartInfo.CreateNoWindow = true;
      processStartInfo.UseShellExecute = false;
      processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      processStartInfo.RedirectStandardError = true;
      processStartInfo.RedirectStandardOutput = true;
      processStartInfo.Arguments = arguments;

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      try
      {
        _log.Info("Executing : " + fileToExecute + " " + arguments);
        using (Process exeProcess = Process.Start(processStartInfo))
        {
          exeProcess.EnableRaisingEvents = true;
          exeProcess.OutputDataReceived += (sender, args) =>
          {
            if (string.IsNullOrEmpty(args.Data) == false)
            {
              _log.Info(args.Data);
            }
          };

          StringBuilder sb = new StringBuilder();

          exeProcess.ErrorDataReceived += (sender, args) =>
          {
            if (string.IsNullOrEmpty(args.Data) == false)
            {
              _log.Error(args.Data);
              sb.AppendLine(args.Data);
            }
          };

          exeProcess.BeginErrorReadLine();
          exeProcess.BeginOutputReadLine();
          exeProcess.WaitForExit();

          if (exeProcess.ExitCode > 0)
          {
            _log.Error(string.Format("Error on executing command line. Error Code : [{0}], Message = [{1}].", exeProcess.ExitCode, sb));
            throw new DbScriptRunnerException(string.Format("Error on executing command line. Error Code : [{0}], Message = [{1}].", exeProcess.ExitCode, sb));
          }
        }
      }
      finally
      {
        stopwatch.Stop();

        _log.InfoFormat("Executing file [{0}] took: {1} s.", fileToExecute, stopwatch.Elapsed.TotalSeconds);
      }
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
  }
}