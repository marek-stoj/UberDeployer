using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.WebApp.Core.Services
{
  public class FunnyGif
  {
    public FunnyGif(string url, string description)
    {
      Guard.NotNullNorEmpty(url, "url");
      Guard.NotNullNorEmpty(description, "description");

      Url = url;
      Description = description;
    }

    public string Url { get; private set; }

    public string Description { get; private set; }
  }
}
