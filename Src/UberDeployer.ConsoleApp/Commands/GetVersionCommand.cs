using System.Collections.Generic;
using System.Linq;
using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core.Management.Metadata;

namespace UberDeployer.ConsoleApp.Commands
{
  public class GetVersionCommand : ConsoleCommand
  {
    public GetVersionCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override string CommandName
    {
      get { return "get-version"; }
    }

    public override int Run(string[] args)
    {
      if (args.Length != 2)
      {
        DisplayCommandUsage();
        return 1;
      }

      string projectName = args[0];
      string targetEnvironmentName = args[1];

      IProjectMetadataExplorer projectMetadataExplorer =
        ObjectFactory.Instance.CreateProjectMetadataExplorer();

      ProjectMetadata projectMetadata =
        projectMetadataExplorer.GetProjectMetadata(
          projectName,
          targetEnvironmentName);

      List<MachineSpecificProjectVersion> projectVersions =
        projectMetadata.ProjectVersions.ToList();

      if (projectVersions.Count == 0)
      {
        OutputWriter.WriteLine("No version info for project \"{0}\" on environment \"{1}\".", projectMetadata.ProjectName, projectMetadata.EnvironmentName);
      }
      else
      {
        foreach (MachineSpecificProjectVersion projectVersion in projectVersions)
        {
          OutputWriter.WriteLine("Version of project \"{0}\" on environment \"{1}\" on machine \"{2}\" is: \"{3}\".", projectMetadata.ProjectName, projectMetadata.EnvironmentName, projectVersion.MachineName, projectVersion.ProjectVersion);
        }
      }

      return 0;
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} project targetEnvironment", CommandName);
    }
  }
}
