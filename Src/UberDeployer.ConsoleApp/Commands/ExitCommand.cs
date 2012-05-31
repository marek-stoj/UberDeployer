using UberDeployer.ConsoleCommander;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ExitCommand : ConsoleCommand
  {
    public ExitCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      return 0;
    }

    public override string CommandName
    {
      get { return "exit"; }
    }
  }
}
