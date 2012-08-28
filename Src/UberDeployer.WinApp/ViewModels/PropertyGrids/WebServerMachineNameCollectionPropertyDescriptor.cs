namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
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
