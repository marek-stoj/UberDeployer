using UberDeployer.ConsoleCommander;

namespace UberDeployer.ConsoleApp.Commands
{
  public class HelpCommand : ConsoleCommand
  {
    public HelpCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override void Run(string[] args)
    {
      if (args.Length == 1)
      {
        string commandName = args[0];

        _commandDispatcher.DisplayCommandUsage(commandName);
      }
      else
      {
        _commandDispatcher.DisplayAvailableCommands();
      }
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
