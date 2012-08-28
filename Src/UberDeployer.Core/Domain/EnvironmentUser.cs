using System;

namespace UberDeployer.Core.Domain
{
  public class EnvironmentUser
  {
    #region Constructor(s)

    public EnvironmentUser(string id, string userName)
    {
      if (string.IsNullOrEmpty(id))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "id");
      }

      if (string.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userName");
      }

      Id = id;
      UserName = userName;
    }

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
      return string.Format("{0}: {1}", Id, UserName);
    }

    #endregion

    #region Properties

    public string Id { get; private set; }

    public string UserName { get; private set; }

    #endregion
  }
}
