using System;
using System.IO;
using System.Xml.Serialization;

namespace UberDeployer.Core.Configuration
{
  public class XmlApplicationConfiguration : IApplicationConfiguration
  {
    #region Nested types

    public class ApplicationConfigurationXml
    {
      public string TeamCityHostName { get; set; }

      public int TeamCityPort { get; set; }

      public string TeamCityUserName { get; set; }

      public string TeamCityPassword { get; set; }

      public string ScExePath { get; set; }

      public string ConnectionString { get; set; }

      public string WebAppInternalApiEndpointUrl { get; set; }

      public int WebAsynchronousPasswordCollectorMaxWaitTimeInSeconds { get; set; }
    }

    private readonly string _xmlFilePath;

    private ApplicationConfigurationXml _applicationConfigurationXml;

    #endregion

    #region Constructor(s)

    public XmlApplicationConfiguration(string xmlFilePath)
    {
      if (string.IsNullOrEmpty(xmlFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "xmlFilePath");
      }

      _xmlFilePath = xmlFilePath;
    }

    #endregion

    #region IApplicationConfiguration Members

    public void Save()
    {
      if (_applicationConfigurationXml == null)
      {
        throw new InvalidOperationException("The XML file has not been loaded yet. Call LoadXmlIfNeeded() first.");
      }

      XmlSerializer xmlSerializer = CreateXmlSerializer();

      using (var fs = File.OpenWrite(_xmlFilePath))
      {
        xmlSerializer.Serialize(fs, _applicationConfigurationXml);
      }
    }

    public string TeamCityHostName
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.TeamCityHostName;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.TeamCityHostName = value;
      }
    }

    public int TeamCityPort
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.TeamCityPort;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.TeamCityPort = value;
      }
    }

    public string TeamCityUserName
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.TeamCityUserName;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.TeamCityUserName = value;
      }
    }

    public string TeamCityPassword
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.TeamCityPassword;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.TeamCityPassword = value;
      }
    }

    public string ScExePath
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.ScExePath;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.ScExePath = value;
      }
    }

    public string ConnectionString
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.ConnectionString;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.ConnectionString = value;
      }
    }

    public string WebAppInternalApiEndpointUrl
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.WebAppInternalApiEndpointUrl;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.WebAppInternalApiEndpointUrl = value;
      }
    }

    public int WebAsynchronousPasswordCollectorMaxWaitTimeInSeconds
    {
      get
      {
        LoadXmlIfNeeded();

        return _applicationConfigurationXml.WebAsynchronousPasswordCollectorMaxWaitTimeInSeconds;
      }

      set
      {
        LoadXmlIfNeeded();

        _applicationConfigurationXml.WebAsynchronousPasswordCollectorMaxWaitTimeInSeconds = value;
      }
    }

    #endregion

    #region Private helper methods

    private static XmlSerializer CreateXmlSerializer()
    {
      return new XmlSerializer(typeof(ApplicationConfigurationXml));
    }

    private void LoadXmlIfNeeded()
    {
      if (_applicationConfigurationXml != null)
      {
        return;
      }

      XmlSerializer xmlSerializer = CreateXmlSerializer();

      using (var fs = File.OpenRead(_xmlFilePath))
      {
        _applicationConfigurationXml = (ApplicationConfigurationXml)xmlSerializer.Deserialize(fs);
      }
    }

    #endregion
  }
}
