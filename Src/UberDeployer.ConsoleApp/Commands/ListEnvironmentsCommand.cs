using System.Collections.Generic;
using System.Linq;
using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core.Domain;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ListEnvironmentsCommand : ConsoleCommand
  {
    public ListEnvironmentsCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      IEnvironmentInfoRepository environmentInfoRepository =
        ObjectFactory.Instance.CreateEnvironmentInfoRepository();

      List<EnvironmentInfo> environmentInfos =
        environmentInfoRepository.GetAll()
          .ToList();

      foreach (EnvironmentInfo environmentInfo in environmentInfos)
      {
        OutputWriter.WriteLine("{0}", environmentInfo.Name);
      }

      return 0;
    }

    public override string CommandName
    {
      get { return "lst-envs"; }
    }
  }
}
