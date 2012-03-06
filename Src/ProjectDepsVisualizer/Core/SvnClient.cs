using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.XPath;

namespace ProjectDepsVisualizer.Core
{
  public class SvnClient : ISvnClient
  {
    private readonly string _svnExeFilePath;
    private readonly string _repositoryBaseUrl;
    private readonly string _userName;
    private readonly string _password;

    #region Constructor(s)

    public SvnClient(string svnExeFilePath, string repositoryBaseUrl, string userName, string password)
    {
      if (svnExeFilePath == null) throw new ArgumentNullException("svnExeFilePath");
      if (repositoryBaseUrl == null) throw new ArgumentNullException("repositoryBaseUrl");
      if (userName == null) throw new ArgumentNullException("userName");
      if (password == null) throw new ArgumentNullException("password");

      _svnExeFilePath = svnExeFilePath;
      _repositoryBaseUrl = repositoryBaseUrl;
      _userName = userName;
      _password = password;
    }

    #endregion

    #region ISvnClient members

    public string GetFileContents(string relativeFilePath)
    {
      if (relativeFilePath == null) throw new ArgumentNullException("relativeFilePath");

      return SvnExportFileContents(relativeFilePath);
    }

    public string GetFileContentsByExt(string relativeDirectory, string extension)
    {
      if (relativeDirectory == null) throw new ArgumentNullException("relativeFilePath");

      return SvnExportFileContentsByExt(relativeDirectory,extension);
    }

    #endregion

    #region Private helper methods

    private string SvnExportFileContents(string relativeFilePath)
    {
      if (relativeFilePath == null) throw new ArgumentNullException("relativeFilePath");

      var processStartInfo =
        new ProcessStartInfo(_svnExeFilePath);

      processStartInfo.Arguments =
        string.Format(
          "--username \"{0}\" --password \"{1}\" {2} \"{3}\"",
          _userName,
          _password,
          "export",
          GetAbsoluteUrl(relativeFilePath));

      processStartInfo.UseShellExecute = false;
      processStartInfo.CreateNoWindow = true;

      using (var process = new Process())
      {
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
      }

      string localFilePath = GetFileName(relativeFilePath);

      if (File.Exists(localFilePath))
      {
        try
        {
          return File.ReadAllText(localFilePath);
        }
        finally
        {
          File.Delete(localFilePath);
        }
      }

      return null;
    }

    private string SvnExportFileContentsByExt(string relativeDirectory, string extension)
    {
      if (relativeDirectory == null) throw new ArgumentNullException("relativeFilePath");

      var processStartInfo =
        new ProcessStartInfo(_svnExeFilePath);

      string tempName = Guid.NewGuid().ToString();
        
      try
      {
        processStartInfo.Arguments =
        string.Format(
          "--username \"{0}\" --password \"{1}\" {2} \"{3}\" --depth=files "+tempName,
          _userName,
          _password,
          "export",
          GetAbsoluteUrl(relativeDirectory));

        processStartInfo.UseShellExecute = false;
        processStartInfo.CreateNoWindow = true;

        using (var process = new Process())
        {
          process.StartInfo = processStartInfo;
          process.Start();
          process.WaitForExit();
        }

        if (Directory.Exists(tempName))
        {
          string fileName = Directory.GetFiles(tempName).FirstOrDefault(f => f.ToLower().Contains(extension));
          if (File.Exists(fileName))
          {

            return File.ReadAllText(fileName);
          }
        }
      }
      finally
      {
        if (Directory.Exists(tempName))
        {
          Directory.Delete(tempName, true);
        }
      }      
      
      return null;
    }

    private static string GetFileName(string relativeFilePath)
    {
      if (relativeFilePath == null) throw new ArgumentNullException("relativeFilePath");

      int indexOfLastSlash = relativeFilePath.LastIndexOf('/');

      return (indexOfLastSlash == -1 ? relativeFilePath : relativeFilePath.Substring(indexOfLastSlash + 1));
    }

    private string GetAbsoluteUrl(string relativeFilePath)
    {
      string baseUrl = _repositoryBaseUrl;

      if (!baseUrl.EndsWith("/") && !relativeFilePath.StartsWith("/"))
      {
        baseUrl += "/";
      }

      return (baseUrl + relativeFilePath);
    }

    #endregion


    public string[] GetRepositoryNames()
    {
      string html = GetWebPage(_repositoryBaseUrl);
      XDocument doc = XDocument.Parse(html);
      List<string> result = new List<string>();
      foreach (XElement xElement in doc.XPathSelectElements("svn/index/dir"))
      {
        result.Add(xElement.Attribute("name").Value);        
      }
      return result.ToArray();
    }

    private string GetWebPage(string _repositoryBaseUrl)
    {
      string output = null;
      HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_repositoryBaseUrl);
      httpRequest.Method = "GET";
      httpRequest.AllowAutoRedirect = true;
      httpRequest.Credentials = new NetworkCredential("nant", "builder");
       

      // if the URI doesn't exist, an exception will be thrown here...
      using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
      {
        using (StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), true))
        {
          output = responseStream.ReadToEnd();
        }
      }

      return output;
    }
  }
}
