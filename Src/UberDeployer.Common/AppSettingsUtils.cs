using System;
using System.Configuration;

namespace UberDeployer.Common
{
  public static class AppSettingsUtils
  {
    public static string ReadAppSettingString(string appSettingKey)
    {
      if (string.IsNullOrEmpty(appSettingKey))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appSettingKey");
      }

      string appSettingValue = ConfigurationManager.AppSettings[appSettingKey];

      if (string.IsNullOrEmpty(appSettingValue))
      {
        throw new ConfigurationErrorsException(
          String.Format("App setting '{0}' was not found in the configuration file.", appSettingKey));
      }

      return appSettingValue;
    }

    public static string ReadAppSettingStringOptional(string appSettingKey)
    {
      if (string.IsNullOrEmpty(appSettingKey))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appSettingKey");
      }

      return ConfigurationManager.AppSettings[appSettingKey];
    }

    public static int ReadAppSettingInt(string appSettingKey)
    {
      string valueString = ReadAppSettingString(appSettingKey);
      int value;

      if (!int.TryParse(valueString, out value))
      {
        throw new ConfigurationErrorsException(
          String.Format("App setting '{0}' could not be parsed as an integer.", appSettingKey));
      }

      return value;
    }

    public static bool ReadAppSettingBool(string appSettingKey)
    {
      string valueString = ReadAppSettingString(appSettingKey);
      bool value;

      if (!bool.TryParse(valueString, out value))
      {
        throw new ConfigurationErrorsException(
          String.Format("App setting '{0}' could not be parsed as a boolean.", appSettingKey));
      }

      return value;
    }

    public static Guid ReadAppSettingGuid(string appSettingKey)
    {
      string valueString = ReadAppSettingString(appSettingKey);
      Guid value;

      try
      {
        value = new Guid(valueString);
      }
      catch (FormatException)
      {
        throw new ConfigurationErrorsException(
          String.Format("App setting '{0}' could not be parsed as a guid.", appSettingKey));
      }

      return value;
    }

    public static bool ContainsAppSetting(string appSettingKey)
    {
      if (string.IsNullOrEmpty(appSettingKey))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appSettingKey");
      }

      string appSettingValue = ConfigurationManager.AppSettings[appSettingKey];

      return !string.IsNullOrEmpty(appSettingValue);
    }

    public static string ReadConnectionString(string connectionStringName)
    {
      if (string.IsNullOrEmpty(connectionStringName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "connectionStringName");
      }

      ConnectionStringSettings connectionStringSettings =
        ConfigurationManager.ConnectionStrings[connectionStringName];

      string connectionString =
        connectionStringSettings != null
          ? connectionStringSettings.ConnectionString
          : null;

      if (string.IsNullOrEmpty(connectionString))
      {
        throw new ConfigurationErrorsException(
          string.Format("Connection string named '{0}' was not found in the configuration file.", connectionStringName));
      }

      return connectionString;
    }
  }
}
