using System;
using System.Net;

namespace UberDeployer.Core.TeamCity
{
  public class Http10WebClient : WebClient
  {
    protected override WebRequest GetWebRequest(Uri address)
    {
      HttpWebRequest httpWebRequest =
        base.GetWebRequest(address) as HttpWebRequest;

      if (httpWebRequest == null)
      {
        throw new Exception("Couldn't get HttpWebRequest.");
      }

      // NOTE: there's a strange behavior in TeamCity that when we use JSON in api, the response
      // gets sent with transfer-encoding: chunked and that is much slower than not using chunked encoding;
      // note also that the problem wouldn't exist if we used XML
      httpWebRequest.ProtocolVersion = HttpVersion.Version10;

      return httpWebRequest;
    }
  }
}
