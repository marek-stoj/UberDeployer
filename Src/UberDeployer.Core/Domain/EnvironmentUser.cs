using System;
using System.ComponentModel;
using UberDeployer.Core.Domain.UI;

namespace UberDeployer.Core.Domain
{
  [TypeConverter(typeof(EnvironmentUserConverter))] // TODO IMM HI: that's for UI!
  public class EnvironmentUser
  {
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

    public override string ToString()
    {
      return string.Format("{0}: {1}", Id, UserName);
    }

    public string Id { get; private set; }

    public string UserName { get; private set; }
  }
}
