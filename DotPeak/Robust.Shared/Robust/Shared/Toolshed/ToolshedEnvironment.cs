// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ToolshedEnvironment
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed;

public sealed class ToolshedEnvironment
{
  [Dependency]
  private readonly IReflectionManager _reflection;
  [Dependency]
  private readonly ILogManager _logManager;
  [Dependency]
  private readonly ToolshedManager _toolshedManager;
  [Dependency]
  private readonly IDependencyCollection _dependency;
  private readonly Dictionary<string, ToolshedCommand> _commands = new Dictionary<string, ToolshedCommand>();
  private List<CommandSpec> _allCommands = new List<CommandSpec>();
  private readonly Dictionary<Type, List<CommandSpec>> _commandTypeMap = new Dictionary<Type, List<CommandSpec>>();
  private readonly Dictionary<Type, List<CommandSpec>> _commandPipeValueMap = new Dictionary<Type, List<CommandSpec>>();
  private readonly Dictionary<CommandSpec, HashSet<Type>> _commandReturnValueMap = new Dictionary<CommandSpec, HashSet<Type>>();
  private readonly Dictionary<Type, CommandSpec[]> _commandTypeCache = new Dictionary<Type, CommandSpec[]>();
  private readonly Dictionary<Type, CompletionOption[]> _commandCompletionCache = new Dictionary<Type, CompletionOption[]>();
  private ISawmill _log;

  public IReadOnlyList<CommandSpec> AllCommands() => (IReadOnlyList<CommandSpec>) this._allCommands;

  public ToolshedCommand GetCommand(string commandName) => this._commands[commandName];

  public bool TryGetCommand(string commandName, [NotNullWhen(true)] out ToolshedCommand? command)
  {
    return this._commands.TryGetValue(commandName, out command);
  }

  public ToolshedEnvironment()
  {
    IoCManager.InjectDependencies<ToolshedEnvironment>(this);
    this.Init(this._reflection.FindTypesWithAttribute<ToolshedCommandAttribute>());
  }

  public ToolshedEnvironment(IEnumerable<Type> commands)
  {
    IoCManager.InjectDependencies<ToolshedEnvironment>(this);
    this.Init(commands);
  }

  private void Init(IEnumerable<Type> commands)
  {
    this._log = this._logManager.GetSawmill("toolshed");
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    foreach (Type command in commands)
    {
      if (!command.IsAssignableTo(typeof (ToolshedCommand)))
      {
        this._log.Error($"The type {command.AssemblyQualifiedName} has {"ToolshedCommandAttribute"} without being a child of {"ToolshedCommand"}");
      }
      else
      {
        ToolshedCommand instance = (ToolshedCommand) Activator.CreateInstance(command);
        this._dependency.InjectDependencies((object) instance, true);
        instance.Init();
        this._commands.Add(instance.Name, instance);
        List<CommandSpec> commandSpecList = new List<CommandSpec>();
        this._commandTypeMap.Add(command, commandSpecList);
        foreach (ToolshedCommandImplementor commandImplementor in instance.CommandImplementors.Values)
        {
          commandSpecList.Add(commandImplementor.Spec);
          this._allCommands.Add(commandImplementor.Spec);
          foreach (CommandMethod method in commandImplementor.Methods)
          {
            Type type1 = method.PipeArg?.ParameterType;
            if ((object) type1 == null)
              type1 = typeof (void);
            Type type2 = type1;
            this.GetTypeImplList(type2).Add(commandImplementor.Spec);
            HashSet<Type> retValuesInternal = this.GetCommandRetValuesInternal(commandImplementor.Spec);
            Type type3;
            if (instance.TryGetReturnType(commandImplementor.SubCommand, type2, (Type[]) null, out type3) || method.Info.ReturnType.Constructable())
            {
              HashSet<Type> typeSet = retValuesInternal;
              Type type4 = type3;
              if ((object) type4 == null)
                type4 = method.Info.ReturnType;
              typeSet.Add(type4);
            }
          }
        }
      }
    }
    this._log.Info($"Initialized new toolshed context in {stopwatch.Elapsed}");
  }

  public bool TryGetCommands<T>([NotNullWhen(true)] out IReadOnlyList<CommandSpec>? commands) where T : ToolshedCommand
  {
    commands = (IReadOnlyList<CommandSpec>) null;
    List<CommandSpec> commandSpecList;
    if (!this._commandTypeMap.TryGetValue(typeof (T), out commandSpecList))
      return false;
    commands = (IReadOnlyList<CommandSpec>) commandSpecList;
    return true;
  }

  internal CommandSpec[] CommandsTakingType(Type? t)
  {
    if ((object) t == null)
      t = typeof (void);
    CommandSpec[] commandSpecArray;
    if (this._commandTypeCache.TryGetValue(t, out commandSpecArray))
      return commandSpecArray;
    Dictionary<(string, string), CommandSpec> dictionary = new Dictionary<(string, string), CommandSpec>();
    foreach (Type allSteppedType in this._toolshedManager.AllSteppedTypes(t))
    {
      foreach (CommandSpec typeImpl in this.GetTypeImplList(allSteppedType))
        dictionary.TryAdd((typeImpl.Cmd.Name, typeImpl.SubCommand), typeImpl);
      if (allSteppedType.IsGenericType)
      {
        foreach (CommandSpec typeImpl in this.GetTypeImplList(allSteppedType.GetGenericTypeDefinition()))
          dictionary.TryAdd((typeImpl.Cmd.Name, typeImpl.SubCommand), typeImpl);
      }
    }
    return this._commandTypeCache[t] = dictionary.Values.ToArray<CommandSpec>();
  }

  public CompletionResult CommandCompletionsForType(Type? t)
  {
    if ((object) t == null)
      t = typeof (void);
    CompletionOption[] options;
    if (!this._commandCompletionCache.TryGetValue(t, out options))
    {
      Dictionary<Type, CompletionOption[]> commandCompletionCache = this._commandCompletionCache;
      Type key = t;
      CommandSpec[] source = this.CommandsTakingType(t);
      CompletionOption[] array;
      CompletionOption[] completionOptionArray = array = ((IEnumerable<CommandSpec>) source).Select<CommandSpec, CompletionOption>((Func<CommandSpec, CompletionOption>) (x => x.AsCompletion())).ToArray<CompletionOption>();
      commandCompletionCache[key] = array;
      options = completionOptionArray;
    }
    return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) options, "<command>");
  }

  public CompletionResult SubCommandCompletionsForType(Type? t, ToolshedCommand command)
  {
    return CompletionResult.FromHintOptions(((IEnumerable<CommandSpec>) this.CommandsTakingType(t)).Where<CommandSpec>((Func<CommandSpec, bool>) (x => x.Cmd == command)).Select<CommandSpec, CompletionOption>((Func<CommandSpec, CompletionOption>) (x => x.AsCompletion())), "<command>");
  }

  public IReadOnlySet<Type> GetCommandRetValues(CommandSpec command)
  {
    return (IReadOnlySet<Type>) this.GetCommandRetValuesInternal(command);
  }

  private HashSet<Type> GetCommandRetValuesInternal(CommandSpec command)
  {
    return this._commandReturnValueMap.GetOrNew<CommandSpec, HashSet<Type>>(command);
  }

  private List<CommandSpec> GetTypeImplList(Type t)
  {
    if (!t.Constructable())
    {
      if (t.IsGenericParameter)
      {
        Type[] parameterConstraints = t.GetGenericParameterConstraints();
        return parameterConstraints.Length != 0 && !((IEnumerable<Type>) parameterConstraints).First<Type>().IsConstructedGenericType ? this.GetTypeImplList(((IEnumerable<Type>) parameterConstraints).First<Type>()) : this.GetTypeImplList(typeof (object));
      }
      t = t.GetGenericTypeDefinition();
    }
    if ((object) t != null && t.IsGenericType && !t.IsConstructedGenericType)
      t = t.GetGenericTypeDefinition();
    return this._commandPipeValueMap.GetOrNew<Type, List<CommandSpec>>(t);
  }
}
