using System.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Common.IO
{
  public class FileAdapter : IFileAdapter
  {
    public bool Exists(string path)
    {
      Guard.NotNullNorEmpty(path, "path");

      return File.Exists(path);
    }

    public void Delete(string path)
    {
      File.Delete(path);
    }

    public void Copy(string sourceFileName, string destFileName)
    {
      File.Copy(sourceFileName, destFileName);
    }
  }
}
