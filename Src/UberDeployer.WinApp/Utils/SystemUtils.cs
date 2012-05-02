using System;
using System.Diagnostics;
using System.IO;

namespace UberDeployer.WinApp.Utils
{
  public static class SystemUtils
  {
    public static void OpenFolder(string folderPath)
    {
      if (string.IsNullOrEmpty(folderPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "folderPath");
      }

      if (!Directory.Exists(folderPath))
      {
        AppUtils.NotifyUserInfo(string.Format("Target folder ('{0}') doesn't exist.", folderPath));
        return;
      }

      Process.Start(folderPath);
    }

    public static void OpenUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "url");
      }

      Process.Start(url);
    }
  }
}
