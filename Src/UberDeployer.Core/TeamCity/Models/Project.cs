namespace UberDeployer.Core.TeamCity.Models
{
  public class Project
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public string Href { get; set; }

    public override string ToString()
    {
      return string.Format("Id: {0}, Name: {1}, Href: {2}", Id, Name, Href);
    }
  }
}
