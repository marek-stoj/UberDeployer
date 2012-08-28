namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class ProjectToFailoverClusterGroupMappingCollectionPropertyDescriptor : CollectionPropertyDescriptor<ProjectToFailoverClusterGroupMappingsCollection>
  {
    #region Constructor(s)

    public ProjectToFailoverClusterGroupMappingCollectionPropertyDescriptor(ProjectToFailoverClusterGroupMappingsCollection collection, int index)
      : base(collection, index, CreatePropertyDescriptorNameSafe(collection, index))
    {
    }

    #endregion

    #region Private helper methods

    private static string CreatePropertyDescriptorNameSafe(ProjectToFailoverClusterGroupMappingsCollection collection, int index)
    {
      if (collection == null || index < 0 || index >= collection.Count)
      {
        return "";
      }

      return collection[index].ProjectName;
    }

    #endregion
  }
}
