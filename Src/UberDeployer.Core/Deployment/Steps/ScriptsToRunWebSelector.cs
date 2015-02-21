using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment.Steps
{
  public class ScriptsToRunWebSelector : IScriptsToRunWebSelector
  {
    private static readonly Dictionary<Guid, string[]> _collectedScriptsByDeploymentId = new Dictionary<Guid, string[]>();
    private readonly string _internalApiEndpointUrl;
    private readonly int _maxWaitTimeInSeconds;

    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    public ScriptsToRunWebSelector(string internalApiEndpointUrl, int maxWaitTimeInSeconds)
    {
      Guard.NotNullNorEmpty(internalApiEndpointUrl, "internalApiEndpointUrl");

      _internalApiEndpointUrl = internalApiEndpointUrl.TrimEnd('/');
      _maxWaitTimeInSeconds = maxWaitTimeInSeconds;
    }

    public static void SetSelectedScriptsToRun(Guid deploymentId, string[] scriptsToRun)
    {
      lock (_collectedScriptsByDeploymentId)
      {
        _collectedScriptsByDeploymentId[deploymentId] = scriptsToRun;
      }
    }

    public string[] GetSelectedScriptsToRun(Guid deploymentId, string[] sourceScriptsList)
    {
      using (var webClient = CreateWebClient())
      {
        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

        var data = new
        {
          DeploymentId = deploymentId,
          ScriptsToRun = sourceScriptsList
        };

        var jsonData = JsonConvert.SerializeObject(data);

        var result = webClient.UploadString(string.Format("{0}/CollectScriptsToRun", _internalApiEndpointUrl), jsonData);

        if (!string.Equals(result, "OK", StringComparison.OrdinalIgnoreCase))
        {
          throw new InternalException("Something went wrong while requesting for database scripts to run.");
        }
      }

      var pollStartTime = DateTime.UtcNow;
      string[] scriptsToRun;

      while (true)
      {
        PostDiagnosticMessage("Waiting for script selection...", DiagnosticMessageType.Trace);

        lock (_collectedScriptsByDeploymentId)
        {
          if (_collectedScriptsByDeploymentId.TryGetValue(deploymentId, out scriptsToRun))
          {
            break;
          }
        }

        Thread.Sleep(1000);

        if (DateTime.UtcNow - pollStartTime > new TimeSpan(0, 0, _maxWaitTimeInSeconds))
        {
          PostDiagnosticMessage("No scripts were selected in the alloted time slot - we'll time out.", DiagnosticMessageType.Trace);

          break;
        }
      }

      if (scriptsToRun == null || scriptsToRun.Length == 0)
      {
        using (var webClient = CreateWebClient())
        {
          webClient.DownloadString(string.Format("{0}/OnCollectScriptsToRunTimedOut?deploymentId={1}", _internalApiEndpointUrl, deploymentId));
        }

        throw new TimeoutException("Given up waiting for scripts selection.");
      }

      PostDiagnosticMessage("Scripts to run were provided - we'll continue.", DiagnosticMessageType.Trace);

      return scriptsToRun;
    }

    private static WebClient CreateWebClient()
    {
      return new WebClient
      {
        UseDefaultCredentials = true
      };
    }

    private void PostDiagnosticMessage(string message, DiagnosticMessageType diagnosticMessageType)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      OnDiagnosticMessagePosted(this, new DiagnosticMessageEventArgs(diagnosticMessageType, message));
    }

    private void OnDiagnosticMessagePosted(object sender, DiagnosticMessageEventArgs diagnosticMessageEventArgs)
    {
      var eventHandler = DiagnosticMessagePosted;

      if (eventHandler != null)
      {
        eventHandler(sender, diagnosticMessageEventArgs);
      }
    }
  }
}