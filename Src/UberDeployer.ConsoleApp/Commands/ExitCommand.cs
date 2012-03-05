using System;
using UberDeployer.ConsoleCommander;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ExitCommand : ConsoleCommand
  {
    public ExitCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override void Run(string[] args)
    {
      Environment.Exit(0);
    }

    public override string CommandName
    {
      get { return "exit"; }
    }
  }
}
