using System;
using System.ComponentModel;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class WebServerMachineNameCollectionPropertyDescriptor : PropertyDescriptor
  {
    private readonly WebServerMachineNameCollection _webServerMachineNameCollection;
    private readonly int _index;

    public WebServerMachineNameCollectionPropertyDescriptor(WebServerMachineNameCollection webServerMachineNameCollection, int index)
      : base(CreatePropertyDescriptorNameSafe(webServerMachineNameCollection, index), null)
    {
      if (webServerMachineNameCollection == null)
      {
        throw new ArgumentNullException("webServerMachineNameCollection");
      }

      _webServerMachineNameCollection = webServerMachineNameCollection;
      _index = index;
    }

    #region Overrides of PropertyDescriptor

    public override bool CanResetValue(object component)
    {
      return false;
    }

    public override object GetValue(object component)
    {
      return _webServerMachineNameCollection[_index];
    }

    public override void ResetValue(object component)
    {
      throw new NotSupportedException();
    }

    public override void SetValue(object component, object value)
    {
      throw new NotSupportedException();
    }

    public override bool ShouldSerializeValue(object component)
    {
      return true;
    }

    public override Type ComponentType
    {
      get { return _webServerMachineNameCollection.GetType(); }
    }

    public override bool IsReadOnly
    {
      get { return true; }
    }

    public override Type PropertyType
    {
      get { return _webServerMachineNameCollection[_index].GetType(); }
    }

    #endregion

    #region Private helper methods

    private static string CreatePropertyDescriptorNameSafe(WebServerMachineNameCollection webServerMachineNameCollection, int index)
    {
      return "Name";
    }

    #endregion
  }
}
