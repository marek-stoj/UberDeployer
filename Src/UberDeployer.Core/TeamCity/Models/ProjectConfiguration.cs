namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfiguration
  {
    #region Overrides of object

    public override string ToString()
    {
      return
        string.Format(
          "Id: {0}, Name: {1}, Href: {2}, WebUrl: {3}, ProjectId: {4}, ProjectName: {5}",
          Id,
          Name,
          Href,
          WebUrl,
          ProjectId,
          ProjectName);
    }

    #endregion

    #region Properties

    public string Id { get; set; }

    public string Name { get; set; }

    public string Href { get; set; }

    public string WebUrl { get; set; }

    public string ProjectId { get; set; }

    public string ProjectName { get; set; }

    #endregion
  }
}
