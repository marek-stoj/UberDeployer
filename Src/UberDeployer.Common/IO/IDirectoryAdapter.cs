namespace UberDeployer.Common.IO
{
  public interface IDirectoryAdapter
  {
    bool Exists(string path);
  }
}