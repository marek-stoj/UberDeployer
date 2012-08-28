using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [ReadOnly(true)]
  public class ProjectToFailoverClusterGroupMappingsCollection : MyCollectionBase
  {
    #region Constructor(s)

    public ProjectToFailoverClusterGroupMappingsCollection(IEnumerable<ProjectToFailoverClusterGroupMappingInPropertyGridViewModel> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }

      foreach (ProjectToFailoverClusterGroupMappingInPropertyGridViewModel item in items)
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
          new ProjectToFailoverClusterGroupMappingCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(propertyDescriptor);
      }

      return propertyDescriptorCollection;
    }

    #endregion

    #region Properties

    public ProjectToFailoverClusterGroupMappingInPropertyGridViewModel this[int index]
    {
      get { return (ProjectToFailoverClusterGroupMappingInPropertyGridViewModel)List[index]; }
    }

    #endregion
  }
}
