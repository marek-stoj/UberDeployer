using System;
using UberDeployer.ConsoleCommander;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ReadEvalPrintLoopCommand : ConsoleCommand
  {
    public ReadEvalPrintLoopCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      OutputWriter.WriteLine("UberDeployer interactive console.");
      OutputWriter.WriteLine("Type 'help' for the list of available commands.");
      OutputWriter.WriteLine();

      do
      {
        try
        {
          OutputWriter.Write("> ");

          string commandLine = Console.ReadLine();

          if (string.IsNullOrEmpty(commandLine))
          {
            continue;
          }

          // TODO IMM HI: split is not enough (what about double quotes?)
          string[] inputArgs = commandLine.Split(' ');

          _commandDispatcher.Dispatch(inputArgs);

          OutputWriter.WriteLine();
        }
        catch (Exception exc)
        {
          OutputWriter.WriteLine(exc);
        }
      }
      while (true);
    }

    public override string CommandName
    {
      get { return "repl"; }
    }
  }
}
