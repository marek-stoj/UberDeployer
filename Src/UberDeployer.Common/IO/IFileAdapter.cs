namespace UberDeployer.Common.IO
{
  public interface IFileAdapter
  {
    bool Exists(string path);
    
    void Delete(string path);

    void Copy(string sourceFileName, string destFileName);
  }
}
