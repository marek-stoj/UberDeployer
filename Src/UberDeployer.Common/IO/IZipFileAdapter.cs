namespace UberDeployer.Common.IO
{
  public interface IZipFileAdapter
  {
    void ExtractAll(string zipFilePath, string targetPath, bool overwriteSilently);
  }
}
