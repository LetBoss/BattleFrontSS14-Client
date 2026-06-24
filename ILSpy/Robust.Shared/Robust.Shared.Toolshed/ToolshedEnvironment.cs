using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

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

	public IReadOnlyList<CommandSpec> AllCommands()
	{
		return _allCommands;
	}

	public ToolshedCommand GetCommand(string commandName)
	{
		return _commands[commandName];
	}

	public bool TryGetCommand(string commandName, [NotNullWhen(true)] out ToolshedCommand? command)
	{
		return _commands.TryGetValue(commandName, out command);
	}

	public ToolshedEnvironment()
	{
		IoCManager.InjectDependencies(this);
		Init(_reflection.FindTypesWithAttribute<ToolshedCommandAttribute>());
	}

	public ToolshedEnvironment(IEnumerable<Type> commands)
	{
		IoCManager.InjectDependencies(this);
		Init(commands);
	}

	private void Init(IEnumerable<Type> commands)
	{
		_log = _logManager.GetSawmill("toolshed");
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		foreach (Type command in commands)
		{
			if (!command.IsAssignableTo(typeof(ToolshedCommand)))
			{
				_log.Error($"The type {command.AssemblyQualifiedName} has {"ToolshedCommandAttribute"} without being a child of {"ToolshedCommand"}");
				continue;
			}
			ToolshedCommand toolshedCommand = (ToolshedCommand)Activator.CreateInstance(command);
			_dependency.InjectDependencies(toolshedCommand, oneOff: true);
			toolshedCommand.Init();
			_commands.Add(toolshedCommand.Name, toolshedCommand);
			List<CommandSpec> list = new List<CommandSpec>();
			_commandTypeMap.Add(command, list);
			foreach (ToolshedCommandImplementor value in toolshedCommand.CommandImplementors.Values)
			{
				list.Add(value.Spec);
				_allCommands.Add(value.Spec);
				CommandMethod[] methods = value.Methods;
				foreach (CommandMethod commandMethod in methods)
				{
					Type type = commandMethod.PipeArg?.ParameterType ?? typeof(void);
					GetTypeImplList(type).Add(value.Spec);
					HashSet<Type> commandRetValuesInternal = GetCommandRetValuesInternal(value.Spec);
					if (toolshedCommand.TryGetReturnType(value.SubCommand, type, null, out Type type2) || commandMethod.Info.ReturnType.Constructable())
					{
						commandRetValuesInternal.Add(type2 ?? commandMethod.Info.ReturnType);
					}
				}
			}
		}
		_log.Info($"Initialized new toolshed context in {stopwatch.Elapsed}");
	}

	public bool TryGetCommands<T>([NotNullWhen(true)] out IReadOnlyList<CommandSpec>? commands) where T : ToolshedCommand
	{
		commands = null;
		if (!_commandTypeMap.TryGetValue(typeof(T), out List<CommandSpec> value))
		{
			return false;
		}
		commands = value;
		return true;
	}

	internal CommandSpec[] CommandsTakingType(Type? t)
	{
		if ((object)t == null)
		{
			t = typeof(void);
		}
		if (_commandTypeCache.TryGetValue(t, out CommandSpec[] value))
		{
			return value;
		}
		Dictionary<(string, string), CommandSpec> dictionary = new Dictionary<(string, string), CommandSpec>();
		foreach (Type item in _toolshedManager.AllSteppedTypes(t))
		{
			foreach (CommandSpec typeImpl in GetTypeImplList(item))
			{
				dictionary.TryAdd((typeImpl.Cmd.Name, typeImpl.SubCommand), typeImpl);
			}
			if (!item.IsGenericType)
			{
				continue;
			}
			foreach (CommandSpec typeImpl2 in GetTypeImplList(item.GetGenericTypeDefinition()))
			{
				dictionary.TryAdd((typeImpl2.Cmd.Name, typeImpl2.SubCommand), typeImpl2);
			}
		}
		return _commandTypeCache[t] = dictionary.Values.ToArray();
	}

	public CompletionResult CommandCompletionsForType(Type? t)
	{
		if ((object)t == null)
		{
			t = typeof(void);
		}
		if (!_commandCompletionCache.TryGetValue(t, out CompletionOption[] value))
		{
			CompletionOption[] array = (_commandCompletionCache[t] = (from x in CommandsTakingType(t)
				select x.AsCompletion()).ToArray());
			value = array;
		}
		return CompletionResult.FromHintOptions(value, "<command>");
	}

	public CompletionResult SubCommandCompletionsForType(Type? t, ToolshedCommand command)
	{
		return CompletionResult.FromHintOptions(from x in CommandsTakingType(t)
			where x.Cmd == command
			select x.AsCompletion(), "<command>");
	}

	public IReadOnlySet<Type> GetCommandRetValues(CommandSpec command)
	{
		return GetCommandRetValuesInternal(command);
	}

	private HashSet<Type> GetCommandRetValuesInternal(CommandSpec command)
	{
		return _commandReturnValueMap.GetOrNew(command);
	}

	private List<CommandSpec> GetTypeImplList(Type t)
	{
		if (!t.Constructable())
		{
			if (t.IsGenericParameter)
			{
				Type[] genericParameterConstraints = t.GetGenericParameterConstraints();
				if (genericParameterConstraints.Length != 0 && !genericParameterConstraints.First().IsConstructedGenericType)
				{
					return GetTypeImplList(genericParameterConstraints.First());
				}
				return GetTypeImplList(typeof(object));
			}
			t = t.GetGenericTypeDefinition();
		}
		if ((object)t != null && t.IsGenericType && !t.IsConstructedGenericType)
		{
			t = t.GetGenericTypeDefinition();
		}
		return _commandPipeValueMap.GetOrNew(t);
	}
}
