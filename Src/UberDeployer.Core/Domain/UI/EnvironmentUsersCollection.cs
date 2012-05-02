using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  [ReadOnly(true)]
  public class EnvironmentUsersCollection : MyCollectionBase
  {
    #region Constructor(s)

    public EnvironmentUsersCollection(IEnumerable<EnvironmentUser> environmentUsers)
    {
      if (environmentUsers == null)
      {
        throw new ArgumentNullException("environmentUsers");
      }

      foreach (EnvironmentUser environmentUser in environmentUsers)
      {
        List.Add(environmentUser);
      }
    }

    #endregion

    #region Overrides of MyCollectionBase

    public override PropertyDescriptorCollection GetProperties()
    {
      var propertyDescriptorCollection = new PropertyDescriptorCollection(null);

      for (int i = 0; i < List.Count; i++)
      {
        var environmentUserCollectionPropertyDescriptor =
          new EnvironmentUserCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(environmentUserCollectionPropertyDescriptor);
      }

      return propertyDescriptorCollection;
    }

    #endregion

    #region Properties

    public EnvironmentUser this[int index]
    {
      get { return (EnvironmentUser)List[index]; }
    }

    #endregion
  }
}
