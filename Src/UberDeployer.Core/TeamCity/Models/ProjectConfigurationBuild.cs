namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationBuild
  {
    #region Overrides of object

    public override string ToString()
    {
      return
        string.Format(
          "Id: {0}, BuildTypeId: {1}, Number: {2}, Status: {3}, WebUrl: {4}",
          Id,
          BuildTypeId,
          Number,
          Status,
          WebUrl);
    }

    #endregion

    #region Properties

    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Number { get; set; }

    public BuildStatus Status { get; set; }

    public string WebUrl { get; set; }
    
    #endregion
  }
}
