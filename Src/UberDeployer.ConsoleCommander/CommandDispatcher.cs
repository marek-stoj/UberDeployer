using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UberDeployer.ConsoleCommander
{
  public class CommandDispatcher
  {
    private readonly TextWriter _outputWriter;

    private readonly Dictionary<string, ConsoleCommand> _consoleCommands;

    #region Constructor(s)

    public CommandDispatcher(TextWriter outputWriter)
    {
      if (outputWriter == null)
      {
        throw new ArgumentNullException("outputWriter");
      }

      _outputWriter = outputWriter;

      _consoleCommands = new Dictionary<string, ConsoleCommand>();
    }

    public CommandDispatcher()
      : this(Console.Out)
    {
    }

    #endregion

    #region Public methods

    public void DiscoverCommands(Assembly assembly)
    {
      if (assembly == null)
      {
        throw new ArgumentNullException("assembly");
      }

      IEnumerable<Type> consoleCommandTypes =
        assembly.GetTypes()
          .Where(t => typeof(ConsoleCommand).IsAssignableFrom(t));

      foreach (Type consoleCommandType in consoleCommandTypes)
      {
        ConsoleCommand consoleCommand =
          (ConsoleCommand)Activator.CreateInstance(consoleCommandType, this);

        _consoleCommands.Add(consoleCommand.CommandName, consoleCommand);
      }
    }

    public void DisplayAvailableCommands()
    {
      OutputWriter.Write("Available commands:");

      if (_consoleCommands.Count == 0)
      {
        OutputWriter.WriteLine(" (none)");
      }
      else
      {
        OutputWriter.WriteLine();

        string[] commandNames = _consoleCommands.Keys.ToArray();

        Array.Sort(commandNames);

        foreach (string commandName in commandNames)
        {
          OutputWriter.WriteLine("  - {0}", commandName);
        }
      }
    }

    public void DisplayCommandUsage(string commandName)
    {
      if (string.IsNullOrEmpty(commandName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "commandName");
      }

      ConsoleCommand consoleCommand = FindCommandByName(commandName);

      if (consoleCommand == null)
      {
        OutputWriter.WriteLine("Unknown command '{0}'.", commandName);

        return;
      }

      consoleCommand.DisplayCommandUsage();
    }

    public int Dispatch(string[] args)
    {
      if (args == null)
      {
        throw new ArgumentNullException("args");
      }

      if (args.Length == 0)
      {
        throw new ArgumentException("There must be at least one argument.", "args");
      }

      string commandName = args[0];
      ConsoleCommand consoleCommand = FindCommandByName(commandName);

      if (consoleCommand == null)
      {
        OutputWriter.WriteLine("Unknown command: '{0}'.", commandName);
        OutputWriter.WriteLine();

        DisplayAvailableCommands();

        return 1;
      }

      string[] followingArgs;

      if (args.Length > 1)
      {
        followingArgs = new string[args.Length - 1];
        args.CopyTo(followingArgs, 1, args.Length - 1);
      }
      else
      {
        followingArgs = new string[0];
      }

      return consoleCommand.Run(followingArgs);
    }

    #endregion

    #region Internal members

    internal TextWriter OutputWriter
    {
      get { return _outputWriter; }
    }

    #endregion

    #region Private helper methods

    private ConsoleCommand FindCommandByName(string commandName)
    {
      if (string.IsNullOrEmpty(commandName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "commandName");
      }

      ConsoleCommand consoleCommand;

      if (_consoleCommands.TryGetValue(commandName, out consoleCommand))
      {
        return consoleCommand;
      }

      return null;
    }

    #endregion
  }
}
