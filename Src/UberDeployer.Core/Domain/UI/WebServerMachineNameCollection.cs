using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  [ReadOnly(true)]
  public class WebServerMachineNameCollection : MyCollectionBase
  {
    #region Constructor(s)

    public WebServerMachineNameCollection(IEnumerable<string> webServerMachineNames)
    {
      if (webServerMachineNames == null)
      {
        throw new ArgumentNullException("webServerMachineNames");
      }

      foreach (string webServerMachineName in webServerMachineNames)
      {
        List.Add(webServerMachineName);
      }
    }

    #endregion

    #region ICustomTypeDescriptor members

    public override PropertyDescriptorCollection GetProperties()
    {
      var propertyDescriptorCollection = new PropertyDescriptorCollection(null);

      for (int i = 0; i < List.Count; i++)
      {
        var environmentUserCollectionPropertyDescriptor =
          new WebServerMachineNameCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(environmentUserCollectionPropertyDescriptor);
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
