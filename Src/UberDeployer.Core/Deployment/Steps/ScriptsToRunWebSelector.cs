using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment.Steps
{
  public class ScriptsToRunWebSelector : IScriptsToRunWebSelector
  {
    private static readonly Dictionary<Guid, string[]> _collectedPasswordByDeploymentId = new Dictionary<Guid, string[]>();
    private readonly string _internalApiEndpointUrl;
    private readonly int _maxWaitTimeInSeconds;

    public ScriptsToRunWebSelector(string internalApiEndpointUrl, int maxWaitTimeInSeconds)
    {
      Guard.NotNullNorEmpty(internalApiEndpointUrl, "internalApiEndpointUrl");

      _internalApiEndpointUrl = internalApiEndpointUrl.TrimEnd('/');
      _maxWaitTimeInSeconds = maxWaitTimeInSeconds;
    }

    public string[] GetSelectedScripts(string[] sourceScriptsList, Guid deploymentId)
    {
      using (var webClient = CreateWebClient())
      {
        var serializeObject = JsonConvert.SerializeObject(sourceScriptsList);

        var result = webClient.UploadString(string.Format("{0}/CollectScriptsToRun", _internalApiEndpointUrl), serializeObject);

        if (!string.Equals(result, "OK", StringComparison.OrdinalIgnoreCase))
        {
          throw new InternalException("Something went wrong while requesting for credentials collection.");
        }
      }

      var pollStartTime = DateTime.UtcNow;
      string[] password;

      while (true)
      {
        PostDiagnosticMessage("Waiting for credentials...", DiagnosticMessageType.Trace);

        lock (_collectedPasswordByDeploymentId)
        {
          if (_collectedPasswordByDeploymentId.TryGetValue(deploymentId, out password))
          {
            break;
          }
        }

        Thread.Sleep(1000);

        if (DateTime.UtcNow - pollStartTime > new TimeSpan(0, 0, _maxWaitTimeInSeconds))
        {
          PostDiagnosticMessage("No credentials were provided in the alloted time slot - we'll time out.", DiagnosticMessageType.Trace);

          break;
        }
      }

      return sourceScriptsList;
    }

    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    private static WebClient CreateWebClient()
    {
      var webClient = new WebClient();

      webClient.UseDefaultCredentials = true;

      return webClient;
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

    public static void SetSelectedScriptsToRun(Guid deploymentId, string[] password)
    {
      lock (_collectedPasswordByDeploymentId)
      {
        _collectedPasswordByDeploymentId[deploymentId] = password;
      }
    }
  }
}