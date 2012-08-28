namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class EnvironmentUserCollectionPropertyDescriptor : CollectionPropertyDescriptor<EnvironmentUsersCollection>
  {
    #region Constructor(s)

    public EnvironmentUserCollectionPropertyDescriptor(EnvironmentUsersCollection collection, int index)
      : base(collection, index, CreatePropertyDescriptorNameSafe(collection, index))
    {
    }

    #endregion

    #region Private methods

    private static string CreatePropertyDescriptorNameSafe(EnvironmentUsersCollection environmentUsersCollection, int index)
    {
      if (environmentUsersCollection == null || index < 0 || index >= environmentUsersCollection.Count)
      {
        return "";
      }

      return environmentUsersCollection[index].Id;
    }

    #endregion
  }
}
