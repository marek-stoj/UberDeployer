using System;
using System.Collections.Generic;
using System.Linq;

namespace UberDeployer.Core.DbDiff
{
  public class DbVersionsModel
  {
    private readonly Dictionary<string, Dictionary<string, HashSet<string>>> _databasesByEnvironment = new Dictionary<string, Dictionary<string, HashSet<string>>>();

    public void AddDatabase(string environmentName, string databaseName, IEnumerable<string> dbVersions)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      if (dbVersions == null)
      {
        throw new ArgumentNullException("dbVersions");
      }

      Dictionary<string, HashSet<string>> databaseNames;

      if (!_databasesByEnvironment.TryGetValue(environmentName, out databaseNames))
      {
        databaseNames = new Dictionary<string, HashSet<string>>();
        _databasesByEnvironment.Add(environmentName, databaseNames);
      }

      databaseNames.Add(databaseName, new HashSet<string>(dbVersions));
    }

    public IEnumerable<string> GetAllSortedEnvironmentNames()
    {
      var allEnvironmentNames = new List<string>(_databasesByEnvironment.Keys);

      allEnvironmentNames.Sort();
      
      return allEnvironmentNames;
    }

    public IEnumerable<string> GetAllSortedDbVersions(string databaseName)
    {
      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      var allDbVersions = new HashSet<string>();

      foreach (string environmentName in _databasesByEnvironment.Keys)
      {
        Dictionary<string, HashSet<string>> databases = _databasesByEnvironment[environmentName];

        if (!databases.ContainsKey(databaseName))
        {
          continue;
        }

        allDbVersions.UnionWith(databases[databaseName]);
      }

      return SortDbVersions(allDbVersions);
    }

    public IEnumerable<string> GetSortedDatabaseNames(string environmentName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      Dictionary<string, HashSet<string>> databaseNames;

      if (!_databasesByEnvironment.TryGetValue(environmentName, out databaseNames))
      {
        return Enumerable.Empty<string>();
      }

      return SortDatabaseNames(databaseNames.Keys);
    }

    public IEnumerable<string> GetAllSortedDatabaseNames()
    {
      var allDatabaseNames = new HashSet<string>();

      foreach (string environmentName in _databasesByEnvironment.Keys)
      {
        allDatabaseNames.UnionWith(_databasesByEnvironment[environmentName].Keys);
      }

      return SortDatabaseNames(allDatabaseNames);
    }

    public bool IsDatabasePresentInEnvironment(string environmentName, string databaseName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      Dictionary<string, HashSet<string>> databaseNames;

      if (!_databasesByEnvironment.TryGetValue(environmentName, out databaseNames))
      {
        return false;
      }

      return databaseNames.ContainsKey(databaseName);
    }

    public bool IsDbVersionPresent(string environmentName, string databaseName, string dbVersion)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      if (string.IsNullOrEmpty(dbVersion))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "dbVersion");
      }

      Dictionary<string, HashSet<string>> databases;

      if (!_databasesByEnvironment.TryGetValue(environmentName, out databases))
      {
        return false;
      }

      HashSet<string> dbVersions;

      if (!databases.TryGetValue(databaseName, out dbVersions))
      {
        return false;
      }

      return dbVersions.Contains(dbVersion);
    }

    public bool AreDatabasesConsistentAcrossEnvironments(string databaseName)
    {
      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      if (_databasesByEnvironment.Keys.All(environmentName => !_databasesByEnvironment[environmentName].ContainsKey(databaseName)))
      {
        // no environment contains this database => this is consistent
        return true;
      }

      if (!_databasesByEnvironment.Keys.All(environmentName => _databasesByEnvironment[environmentName].ContainsKey(databaseName)))
      {
        // not all environments contain this database => this is not consistent because here we know that at least one environment contains this database
        return false;
      }

      return
        _databasesByEnvironment.Keys
          .All(
            environmentName1 =>
            _databasesByEnvironment.Keys.All(
              environmentName2 =>
              environmentName1 == environmentName2 ||
              _databasesByEnvironment[environmentName1][databaseName].SetEquals(_databasesByEnvironment[environmentName2][databaseName])));
    }

    private static IEnumerable<string> SortDatabaseNames(IEnumerable<string> databaseNames)
    {
      var sortedDatabaseNames = new List<string>(databaseNames);

      sortedDatabaseNames.Sort();

      return sortedDatabaseNames;
    }

    private static IEnumerable<string> SortDbVersions(IEnumerable<string> allDbVersions)
    {
      var sortedDbVersions = new List<string>(allDbVersions);

      // TODO IMM HI: sort by major, minor etc.
      sortedDbVersions.Sort(
        (dbVersion1String, dbVersion2String) =>
          {
            DbVersion dbVersion1;
            DbVersion dbVersion2;

            try
            {
              dbVersion1 = DbVersion.FromString(dbVersion1String);
            }
            catch (FormatException)
            {
              dbVersion1 = null;
            }

            try
            {
              dbVersion2 = DbVersion.FromString(dbVersion2String);
            }
            catch (FormatException)
            {
              dbVersion2 = null;
            }

            if (dbVersion1 == null && dbVersion2 == null)
            {
              return 0;
            }

            if (dbVersion1 == null && dbVersion2 != null)
            {
              return -1;
            }

            if (dbVersion1 != null && dbVersion2 == null)
            {
              return 1;
            }

            return dbVersion1.IsSmallerThan(dbVersion2) ? -1 : 1;
          });

      return sortedDbVersions;
    }
  }
}
