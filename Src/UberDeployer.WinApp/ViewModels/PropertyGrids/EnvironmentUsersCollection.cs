using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [ReadOnly(true)]
  public class EnvironmentUsersCollection : MyCollectionBase
  {
    #region Constructor(s)

    public EnvironmentUsersCollection(IEnumerable<EnvironmentUserInPropertyGridVieModel> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }

      foreach (EnvironmentUserInPropertyGridVieModel item in items)
      {
        List.Add(item);
      }
    }

    #endregion

    #region Overrides of MyCollectionBase

    public override PropertyDescriptorCollection GetProperties()
    {
      var propertyDescriptorCollection = new PropertyDescriptorCollection(null);

      for (int i = 0; i < List.Count; i++)
      {
        var propertyDescriptor =
          new EnvironmentUserCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(propertyDescriptor);
      }

      return propertyDescriptorCollection;
    }

    #endregion

    #region Properties

    public EnvironmentUserInPropertyGridVieModel this[int index]
    {
      get { return (EnvironmentUserInPropertyGridVieModel)List[index]; }
    }

    #endregion
  }
}
