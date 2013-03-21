using System;
using System.Diagnostics;
using System.IO;

namespace UberDeployer.Core.Deployment
{
  public class ConfigureBinariesStep : DeploymentStep
  {
    private readonly string _templateConfigurationName;
    private readonly string _artifactsDirPath;

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

    public override string Description
    {
      get { return string.Format("Run Config_{0}.bat in order to create environment-specific artifacts.", _templateConfigurationName); }
    }

    private void Execute(string fileToExecute, string workingDir, string arguments)
    {
      var processStartInfo =
        new ProcessStartInfo
        {
          FileName = fileToExecute,
          WorkingDirectory = workingDir,
          CreateNoWindow = true,
          UseShellExecute = false,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          Arguments = arguments,
        };

      var stopwatch = new Stopwatch();

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
