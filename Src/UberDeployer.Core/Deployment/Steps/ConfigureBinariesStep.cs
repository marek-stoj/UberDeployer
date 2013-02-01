using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class ConfigureBinariesStep : DeploymentStep
  {
    private readonly string _templateConfigurationName;

    private readonly string _artifactsDirPath;

    public override string Description
    {
      get { return "Runs *.bat for specific configuration."; }
    }

    public ConfigureBinariesStep(string templateConfigurationName, string artifactsDirPath)
    {
      if (string.IsNullOrWhiteSpace(artifactsDirPath))
      {
        throw new ArgumentNullException("artifactsDirPath");
      }

      if (string.IsNullOrWhiteSpace(templateConfigurationName))
      {
        throw new ArgumentNullException("templateConfigurationName");
      }

      _templateConfigurationName = templateConfigurationName;
      _artifactsDirPath = artifactsDirPath;
    }

    protected override void DoExecute()
    {
      Execute(Path.Combine(_artifactsDirPath, string.Format("Config_{0}.bat", _templateConfigurationName)), _artifactsDirPath, null);
    }

    private void Execute(string fileToExecute, string workingDir, string arguments)
    {
      ProcessStartInfo processStartInfo = new ProcessStartInfo();
      processStartInfo.FileName = fileToExecute;
      processStartInfo.WorkingDirectory = workingDir;
      processStartInfo.CreateNoWindow = true;
      processStartInfo.UseShellExecute = false;
      processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      processStartInfo.RedirectStandardError = true;
      processStartInfo.RedirectStandardOutput = true;
      processStartInfo.Arguments = arguments;

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      bool errorOccured = false;

      try
      {
        using (Process exeProcess = Process.Start(processStartInfo))
        {
          exeProcess.EnableRaisingEvents = true;
          exeProcess.OutputDataReceived += (sender, args) =>
          {
            if (string.IsNullOrEmpty(args.Data) == false)
            {
              PostDiagnosticMessage(args.Data, DiagnosticMessageType.Trace);
            }
          };

          exeProcess.ErrorDataReceived += (sender, args) =>
          {
            if (string.IsNullOrEmpty(args.Data) == false)
            {
              PostDiagnosticMessage(args.Data, DiagnosticMessageType.Error);
              errorOccured = true;
            }
          };

          exeProcess.BeginErrorReadLine();
          exeProcess.BeginOutputReadLine();

          exeProcess.WaitForExit();

          if (exeProcess.ExitCode > 0 || errorOccured)
          {
            throw new InvalidOperationException("Error on executing command line.");
          }
        }
      }
      finally
      {
        stopwatch.Stop();

        PostDiagnosticMessage(string.Format("Executing file [{0}] took: {1} s.", fileToExecute, stopwatch.Elapsed.TotalSeconds), DiagnosticMessageType.Info);
      }
    }
  }
}