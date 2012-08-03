namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class WebServerMachineNameCollectionPropertyDescriptor : CollectionPropertyDescriptor<WebServerMachineNameCollection>
  {
    #region Constructor(s)

    public WebServerMachineNameCollectionPropertyDescriptor(WebServerMachineNameCollection collection, int index)
      : base(collection, index, "Name")
    {
    }

    #endregion
  }
}
