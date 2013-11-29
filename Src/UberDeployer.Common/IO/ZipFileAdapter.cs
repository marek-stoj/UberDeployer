using Ionic.Zip;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Common.IO
{
  public class ZipFileAdapter : IZipFileAdapter
  {
    public void ExtractAll(string zipFilePath, string targetPath, bool overwriteSilently)
    {
      Guard.NotNullNorEmpty(zipFilePath, "zipFilePath");
      Guard.NotNullNorEmpty(targetPath, "targetPath");

      using (var zipFile = new ZipFile(zipFilePath))
      {
        ExtractExistingFileAction extractExistingFileAction =
          overwriteSilently
            ? ExtractExistingFileAction.OverwriteSilently
            : ExtractExistingFileAction.Throw;

        zipFile.ExtractAll(
          targetPath,
          extractExistingFileAction);
      }
    }
  }
}
