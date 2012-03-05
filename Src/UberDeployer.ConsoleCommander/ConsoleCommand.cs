using System;
using System.IO;

namespace UberDeployer.ConsoleCommander
{
  public abstract class ConsoleCommand
  {
    protected readonly CommandDispatcher _commandDispatcher;

    protected ConsoleCommand(CommandDispatcher commandDispatcher)
    {
      if (commandDispatcher == null)
      {
        throw new ArgumentNullException("commandDispatcher");
      }

      _commandDispatcher = commandDispatcher;
    }

    public abstract void Run(string[] args);

    public virtual void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0}", CommandName);
    }

    public abstract string CommandName { get; }

    protected TextWriter OutputWriter
    {
      get { return _commandDispatcher.OutputWriter; }
    }
  }
}
