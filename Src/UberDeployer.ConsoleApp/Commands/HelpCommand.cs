using UberDeployer.ConsoleCommander;

namespace UberDeployer.ConsoleApp.Commands
{
  public class HelpCommand : ConsoleCommand
  {
    public HelpCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      if (args.Length == 1)
      {
        string commandName = args[0];

        _commandDispatcher.DisplayCommandUsage(commandName);

        return 1;
      }
      
      _commandDispatcher.DisplayAvailableCommands();

      return 0;
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} [commandName]", CommandName);
    }

    public override string CommandName
    {
      get { return "help"; }
    }
  }
}
