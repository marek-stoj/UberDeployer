using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace UberDeployer.Core.Management.MsDeploy
{
  public class MsDeploy : IMsDeploy
  {
    private static readonly CultureInfo _EnUsCultureInfo = new CultureInfo("en-US");

    private readonly ConstructorInfo _msDeployCtor;
    private readonly MethodInfo _msDeployExecuteMethod;
    private readonly MethodInfo _displayExceptionMethod;

    #region Constructor(s)

    public MsDeploy(string msDeployExeAbsolutePath)
    {
      if (string.IsNullOrEmpty(msDeployExeAbsolutePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "msDeployExeAbsolutePath");
      }

      if (!Path.IsPathRooted(msDeployExeAbsolutePath))
      {
        throw new ArgumentException("The path must be an absolute path.", "msDeployExeAbsolutePath");
      }

      if (!File.Exists(msDeployExeAbsolutePath))
      {
        throw new FileNotFoundException(string.Format("Specified msdeploy exe file ('{0}') doesn't exist.", msDeployExeAbsolutePath), msDeployExeAbsolutePath);
      }

      Assembly assembly = Assembly.LoadFile(msDeployExeAbsolutePath);

      if (assembly != null)
      {
        Type[] types = assembly.GetTypes();

        Type msDeployType =
          types.SingleOrDefault(t => t.Name == "MSDeploy");

        if (msDeployType != null)
        {
          _msDeployCtor =
            msDeployType.GetConstructor(
              BindingFlags.NonPublic | BindingFlags.Instance,
              Type.DefaultBinder,
              new[] { typeof(bool), typeof(bool), typeof(bool), typeof(IEnumerable<string>) },
              null);

          _msDeployExecuteMethod =
            msDeployType.GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        Type commandLineHelperType =
          types.SingleOrDefault(t => t.Name == "CommandLineHelper");

        if (commandLineHelperType != null)
        {
          _displayExceptionMethod =
            commandLineHelperType.GetMethod("DisplayException", BindingFlags.Public | BindingFlags.Static);
        }
      }

      if (_msDeployCtor == null)
      {
        throw new Exception("Couldn't obtain the constructor of the MSDeploy type.");
      }

      if (_msDeployExecuteMethod == null)
      {
        throw new Exception("Couldn't obtain the Execute() method of the MSDeploy type.");
      }

      if (_displayExceptionMethod == null)
      {
        throw new Exception("Couldn't obtain the DisplayException method of the CommandLineHelper type.");
      }
    }

    #endregion

    #region Public methods

    public void Run(string[] args, out string consoleOutput)
    {
      if (args == null)
      {
        throw new ArgumentNullException("args");
      }

      TextWriter oldConsoleOut = null;
      TextWriter oldConsoleError = null;

      try
      {
        oldConsoleOut = Console.Out;
        oldConsoleError = Console.Error;

        var consoleOutputStringBuilder = new StringBuilder();
        var consoleErrorStringBuilder = new StringBuilder();

        using (var consoleOutputStringWriter = new StringWriter(consoleOutputStringBuilder))
        using (var consoleErrorStringWriter = new StringWriter(consoleErrorStringBuilder))
        {
          Console.SetOut(consoleOutputStringWriter);
          Console.SetError(consoleErrorStringWriter);

          var newArgs = new List<string>(args);

          newArgs.Add("-verbose");

          object msDeployObject = CreateMsDeployObject(newArgs);

          var oldCulture = Thread.CurrentThread.CurrentCulture;
          var oldUICulture = Thread.CurrentThread.CurrentUICulture;

          try
          {
            Thread.CurrentThread.CurrentCulture = _EnUsCultureInfo;
            Thread.CurrentThread.CurrentUICulture = _EnUsCultureInfo;

            _msDeployExecuteMethod.Invoke(msDeployObject, null);
          }
          catch (TargetInvocationException exc)
          {
            _displayExceptionMethod.Invoke(null, new object[] { exc.InnerException, false });

            throw new MsDeployException(exc.InnerException, consoleErrorStringBuilder.ToString());
          }
          finally
          {
            Thread.CurrentThread.CurrentCulture = oldCulture;
            Thread.CurrentThread.CurrentUICulture = oldUICulture;
          }
        }

        // TODO IMM HI: log console output (log4net)?
        // TODO IMM HI: log console error (log4net)?

        consoleOutput = consoleOutputStringBuilder.ToString();

        string consoleError = consoleErrorStringBuilder.ToString();

        if (!string.IsNullOrEmpty(consoleError))
        {
          throw new MsDeployException(consoleError);
        }
      }
      finally
      {
        if (oldConsoleOut != null)
        {
          Console.SetOut(oldConsoleOut);
        }

        if (oldConsoleError != null)
        {
          Console.SetError(oldConsoleError);
        }
      }
    }

    public void CreateIisAppManifestFile(string localWebAppPath, string outMsDeployManifestFilePath)
    {
      // TODO IMM HI: move to resource?
      // TODO IMM HI: do we really need setAcl?
      const string manifestTemplate =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
        "<sitemanifest>" +
        "  <IisApp path=\"{0}\" />" +
        "  <setAcl path=\"{0}\" setAclResourceType=\"Directory\" />" +
        "  <setAcl path=\"{0}\" setAclUser=\"anonymousAuthenticationUser\" setAclResourceType=\"Directory\" />" +
        "</sitemanifest>";

      File.WriteAllText(outMsDeployManifestFilePath, string.Format(manifestTemplate, localWebAppPath));
    }

    #endregion

    #region Private helper methods

    private object CreateMsDeployObject(IEnumerable<string> args)
    {
      object msDeployObject;

      try
      {
        msDeployObject =
          _msDeployCtor.Invoke(
            new object[]
              {
                false, // debug
                false, // trace
                false, // help
                args, // command line args
              });
      }
      catch (TargetInvocationException exc)
      {
        throw new MsDeployException(exc);
      }

      return msDeployObject;
    }

    #endregion
  }
}
