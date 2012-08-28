using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [ReadOnly(true)]
  public class WebServerMachineNameCollection : MyCollectionBase
  {
    #region Constructor(s)

    public WebServerMachineNameCollection(IEnumerable<string> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }

      foreach (string item in items)
      {
        List.Add(item);
      }
    }

    #endregion

    #region ICustomTypeDescriptor members

    public override PropertyDescriptorCollection GetProperties()
    {
      var propertyDescriptorCollection = new PropertyDescriptorCollection(null);

      for (int i = 0; i < List.Count; i++)
      {
        var propertyDescriptor =
          new WebServerMachineNameCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(propertyDescriptor);
      }

      return propertyDescriptorCollection;
    }

    #endregion

    #region Properties

    public string this[int index]
    {
      get { return (string)List[index]; }
    }

    #endregion
  }
}
