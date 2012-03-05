using System;

namespace UberDeployer.Core.Domain
{
  public class IisAppPoolInfo
  {
    #region Constructor(s)

    public IisAppPoolInfo(string name, IisAppPoolVersion version, IisAppPoolMode mode)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }

      Name = name;
      Version = version;
      Mode = mode;
    }

    #endregion

    #region Overrides of object

    public override string ToString()
    {
      return string.Format("Name: '{0}'. Version: '{1}'. Mode: '{2}'.", Name, Version, Mode);
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    public IisAppPoolVersion Version { get; private set; }

    public IisAppPoolMode Mode { get; private set; }

    #endregion
  }
}
