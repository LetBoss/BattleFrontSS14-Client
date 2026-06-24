using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Reflection;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Invocation;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class ToolshedManager
{
	[Dependency]
	private readonly IDynamicTypeFactoryInternal _typeFactory;

	[Dependency]
	private readonly IEntityManager _entity;

	[Dependency]
	private readonly IReflectionManager _reflection;

	[Dependency]
	private readonly ILogManager _logManager;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly ISharedPlayerManager _player;

	[Dependency]
	private readonly IConsoleHost _conHost;

	private ISawmill _log;

	private Dictionary<NetUserId, OldShellInvocationContext> _contexts = new Dictionary<NetUserId, OldShellInvocationContext>();

	private readonly Dictionary<Type, ITypeParser?> _consoleTypeParsers = new Dictionary<Type, ITypeParser>();

	private readonly Dictionary<ITypeParser, ITypeParser?> _argParsers = new Dictionary<ITypeParser, ITypeParser>();

	private readonly Dictionary<Type, ITypeParser> _customParsers = new Dictionary<Type, ITypeParser>();

	private readonly Dictionary<Type, Type> _genericTypeParsers = new Dictionary<Type, Type>();

	private readonly List<(Type, Type)> _constrainedParsers = new List<(Type, Type)>();

	private ToolshedEnvironment? _defaultEnvironment;

	private int _maxOutput = 128;

	private Dictionary<Type, HashSet<Type>> _typeCache = new Dictionary<Type, HashSet<Type>>();

	public IPermissionController? ActivePermissionController { get; set; }

	public ToolshedEnvironment DefaultEnvironment
	{
		get
		{
			if (_net.IsClient)
			{
				throw new NotImplementedException("Toolshed is not yet ready for client-side use.");
			}
			if (_defaultEnvironment == null)
			{
				_defaultEnvironment = new ToolshedEnvironment();
			}
			return _defaultEnvironment;
		}
	}

	public void Initialize()
	{
		_log = _logManager.GetSawmill("toolshed");
		InitializeParser();
		_player.PlayerStatusChanged += OnStatusChanged;
	}

	private void OnStatusChanged(object? sender, SessionStatusEventArgs e)
	{
		if (_contexts.TryGetValue(e.Session.UserId, out OldShellInvocationContext value))
		{
			if (e.NewStatus == SessionStatus.Disconnected)
			{
				value.Shell = null;
			}
			if (e.NewStatus == SessionStatus.InGame)
			{
				value.Shell = new ConsoleShell(_conHost, e.Session, isLocal: false);
			}
		}
	}

	public bool InvokeCommand(ICommonSession session, string command, object? input, out object? result)
	{
		if (!_contexts.TryGetValue(session.UserId, out OldShellInvocationContext value))
		{
			ConsoleShell shell = new ConsoleShell(_conHost, session, isLocal: false);
			value = (_contexts[session.UserId] = new OldShellInvocationContext(shell));
		}
		value.ClearErrors();
		return InvokeCommand(value, command, input, out result);
	}

	public bool InvokeCommand(IConsoleShell session, string command, object? input, out object? result, out IInvocationContext ctx)
	{
		NetUserId key = session.Player?.UserId ?? default(NetUserId);
		if (!_contexts.TryGetValue(key, out OldShellInvocationContext value))
		{
			value = new OldShellInvocationContext(session);
			_contexts[key] = value;
		}
		value.ClearErrors();
		ctx = value;
		return InvokeCommand(ctx, command, input, out result);
	}

	public bool InvokeCommand(IInvocationContext ctx, string command, object? input, out object? result)
	{
		ctx.ClearErrors();
		result = null;
		ParserContext parserContext = new ParserContext(command, this, ctx);
		if (!CommandRun.TryParse(parserContext, input?.GetType(), null, out CommandRun expr))
		{
			ctx.ReportError((IConError)(parserContext.Error ?? ((object)new FailedToParseError())));
			return false;
		}
		result = expr.Invoke(input, ctx);
		return true;
	}

	public CompletionResult? GetCompletions(ConsoleShell shell, string command)
	{
		NetUserId key = shell.Player?.UserId ?? default(NetUserId);
		if (!_contexts.TryGetValue(key, out OldShellInvocationContext value))
		{
			OldShellInvocationContext oldShellInvocationContext = (_contexts[key] = new OldShellInvocationContext(shell));
			value = oldShellInvocationContext;
		}
		return GetCompletions(value, command);
	}

	public CompletionResult? GetCompletions(IInvocationContext ctx, string command)
	{
		ctx.ClearErrors();
		ParserContext obj = new ParserContext(command, this, ctx)
		{
			GenerateCompletions = true
		};
		CommandRun.TryParse(obj, null, null, out CommandRun _);
		return obj.Completions;
	}

	private void InitializeParser()
	{
		foreach (Type allChild in _reflection.GetAllChildren<ITypeParser>())
		{
			Type baseType = allChild.BaseType;
			bool flag = false;
			while (baseType != null)
			{
				if (baseType.IsGenericType(typeof(TypeParser<>)))
				{
					flag = true;
					break;
				}
				baseType = baseType.BaseType;
			}
			if (!flag)
			{
				continue;
			}
			if (allChild.IsGenericType)
			{
				Type type = allChild.BaseType.GetGenericArguments().First();
				if (type.IsGenericType)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					if (!_genericTypeParsers.TryAdd(genericTypeDefinition, allChild))
					{
						throw new Exception($"Duplicate toolshed type parser for type: {genericTypeDefinition}");
					}
					_log.Verbose("Setting up " + allChild.PrettyName() + ", " + type.GetGenericTypeDefinition().PrettyName());
				}
				else if (type.IsGenericParameter)
				{
					_constrainedParsers.Add((type, allChild));
					_log.Verbose("Setting up " + allChild.PrettyName() + ", for T where T: " + string.Join(", ", from x in type.GetGenericParameterConstraints()
						select x.PrettyName()));
				}
			}
			else
			{
				ITypeParser typeParser = (ITypeParser)_typeFactory.CreateInstanceUnchecked(allChild, oneOff: true);
				if (typeParser is IPostInjectInit postInjectInit)
				{
					postInjectInit.PostInject();
				}
				_log.Verbose("Setting up " + allChild.PrettyName() + ", " + typeParser.Parses.PrettyName());
				if (!_consoleTypeParsers.TryAdd(typeParser.Parses, typeParser))
				{
					throw new Exception($"Discovered conflicting parsers for type {typeParser.Parses.PrettyName()}: {allChild.PrettyName()} and {_consoleTypeParsers[typeParser.Parses].GetType().PrettyName()}");
				}
			}
		}
	}

	internal ITypeParser? GetParserForType(Type t)
	{
		if (_consoleTypeParsers.TryGetValue(t, out ITypeParser value))
		{
			return value;
		}
		value = FindParserForType(t);
		_consoleTypeParsers.TryAdd(t, value);
		return value;
	}

	internal ITypeParser? GetArgumentParser(Type t)
	{
		ITypeParser parserForType = GetParserForType(t);
		if (parserForType != null)
		{
			return GetArgumentParser(parserForType);
		}
		return GetParserForType(typeof(ValueRef<>).MakeGenericType(t));
	}

	internal ITypeParser? GetArgumentParser(ITypeParser baseParser)
	{
		if (!baseParser.EnableValueRef)
		{
			return baseParser;
		}
		if (_argParsers.TryGetValue(baseParser, out ITypeParser value))
		{
			return value;
		}
		Type parses = baseParser.Parses;
		value = ((parses.IsValueRef() || parses.IsAssignableTo(typeof(Block))) ? baseParser : ((!baseParser.GetType().HasGenericParent(typeof(TypeParser<>))) ? GetCustomParser(typeof(CustomValueRefTypeParser<, >).MakeGenericType(parses, baseParser.GetType())) : GetParserForType(typeof(ValueRef<>).MakeGenericType(parses))));
		return _argParsers[baseParser] = value;
	}

	internal TParser GetCustomParser<TParser, T>() where TParser : CustomTypeParser<T>, new() where T : notnull
	{
		return (TParser)GetCustomParser(typeof(TParser));
	}

	internal ITypeParser GetCustomParser(Type parser)
	{
		if (_customParsers.TryGetValue(parser, out ITypeParser value))
		{
			return value;
		}
		if (parser.ContainsGenericParameters)
		{
			throw new ArgumentException("Type cannot contain generic parameters");
		}
		if (!parser.IsCustomParser())
		{
			throw new ArgumentException(parser.PrettyName() + " does not inherit from " + typeof(CustomTypeParser<>).PrettyName());
		}
		value = (ITypeParser)_typeFactory.CreateInstanceUnchecked(parser, oneOff: true);
		if (value is IPostInjectInit postInjectInit)
		{
			postInjectInit.PostInject();
		}
		return _customParsers[parser] = value;
	}

	private ITypeParser? FindParserForType(Type t)
	{
		if (t.IsConstructedGenericType && _genericTypeParsers.TryGetValue(t.GetGenericTypeDefinition(), out Type value))
		{
			try
			{
				Type type = value.MakeGenericType(t.GenericTypeArguments);
				ITypeParser obj = (ITypeParser)_typeFactory.CreateInstanceUnchecked(type, oneOff: true);
				if (obj is IPostInjectInit postInjectInit)
				{
					postInjectInit.PostInject();
				}
				return obj;
			}
			catch (SecurityException)
			{
				_log.Info("Couldn't use " + value.PrettyName() + " to parse " + t.PrettyName());
			}
		}
		foreach (var constrainedParser in _constrainedParsers)
		{
			Type item = constrainedParser.Item2;
			try
			{
				Type type2 = item.MakeGenericType(t);
				ITypeParser obj2 = (ITypeParser)_typeFactory.CreateInstanceUnchecked(type2, oneOff: true);
				if (obj2 is IPostInjectInit postInjectInit2)
				{
					postInjectInit2.PostInject();
				}
				return obj2;
			}
			catch (SecurityException)
			{
			}
			catch (ArgumentException)
			{
			}
		}
		Type baseType = t.BaseType;
		if ((object)baseType != null && baseType != typeof(object) && baseType != typeof(ValueType))
		{
			ITypeParser parserForType = GetParserForType(baseType);
			if (parserForType != null)
			{
				return parserForType;
			}
		}
		Type[] interfaces = t.GetInterfaces();
		foreach (Type t2 in interfaces)
		{
			ITypeParser parserForType2 = GetParserForType(t2);
			if (parserForType2 != null)
			{
				return parserForType2;
			}
		}
		return null;
	}

	public bool TryParse<T>(ParserContext parserContext, [NotNullWhen(true)] out T? parsed)
	{
		Type typeFromHandle = typeof(T);
		typeFromHandle = Nullable.GetUnderlyingType(typeFromHandle) ?? typeFromHandle;
		object parsed2;
		bool result = TryParse(parserContext, typeFromHandle, out parsed2);
		if (parsed2 != null)
		{
			parsed = (T)parsed2;
			return result;
		}
		parsed = default(T);
		return result;
	}

	public CompletionResult? TryAutocomplete(ParserContext ctx, Type t, CommandArgument? arg)
	{
		return GetParserForType(t)?.TryAutocomplete(ctx, arg);
	}

	public bool TryParse(ParserContext parserContext, Type t, [NotNullWhen(true)] out object? parsed)
	{
		parsed = null;
		ITypeParser parserForType = GetParserForType(t);
		if (parserForType == null)
		{
			if (!parserContext.GenerateCompletions)
			{
				parserContext.Error = new UnparseableValueError(t);
			}
			return false;
		}
		if (!parserForType.TryParse(parserContext, out parsed))
		{
			return false;
		}
		return true;
	}

	public bool TryParseArgument(ParserContext parserContext, Type t, [NotNullWhen(true)] out object? parsed)
	{
		parsed = null;
		return GetArgumentParser(t)?.TryParse(parserContext, out parsed) ?? false;
	}

	public bool CheckInvokable(CommandSpec command, ICommonSession? session, out IConError? error)
	{
		IPermissionController activePermissionController = ActivePermissionController;
		if (activePermissionController != null)
		{
			return activePermissionController.CheckInvokable(command, session, out error);
		}
		error = null;
		return true;
	}

	public string PrettyPrintType(object? value, out IEnumerable? more, bool moreUsed = false, int? maxOutput = null)
	{
		int valueOrDefault = maxOutput.GetValueOrDefault();
		if (!maxOutput.HasValue)
		{
			valueOrDefault = _maxOutput;
			maxOutput = valueOrDefault;
		}
		more = null;
		if (value == null)
		{
			return "";
		}
		if (value is IToolshedPrettyPrint toolshedPrettyPrint)
		{
			return toolshedPrettyPrint.PrettyPrint(this, out more, moreUsed, maxOutput);
		}
		if (value is string text)
		{
			if (text.Length > 32768)
			{
				return text.Substring(0, 32768) + "(refusing to output more!)";
			}
			return text;
		}
		if (value is FormattedMessage formattedMessage)
		{
			return formattedMessage.ToMarkup();
		}
		if (value is EntityUid entityUid)
		{
			return _entity.ToPrettyString(entityUid);
		}
		if (value is Type type)
		{
			return type.PrettyName();
		}
		if (value.GetType().IsAssignableTo(typeof(IDictionary)))
		{
			IDictionaryEnumerator enumerator = ((IDictionary)value).GetEnumerator();
			List<string> list = new List<string>();
			while (enumerator.MoveNext())
			{
				list.Add("(" + PrettyPrintType(enumerator.Key, out IEnumerable more2) + ", " + PrettyPrintType(enumerator.Value, out more2));
			}
			return "Dictionary {\n" + string.Join(",\n", list) + "\n}";
		}
		if (value is IEnumerable source)
		{
			List<object> list2 = source.Cast<object>().ToList();
			if (list2.Count > maxOutput.Value)
			{
				more = list2.GetRange(maxOutput.Value, list2.Count - maxOutput.Value - 1);
			}
			string text2 = string.Join(",\n", from x in list2.Take(maxOutput.Value)
				select PrettyPrintType(x, out IEnumerable _));
			if (more != null && moreUsed)
			{
				return text2 + "... (output truncated, run more for further output)";
			}
			if (more != null)
			{
				return text2 + "... (output truncated, if possible tee the value into it's own variable)";
			}
			return text2;
		}
		return value.ToString() ?? "[unrepresentable]";
	}

	internal IEnumerable<Type> AllSteppedTypes(Type t, bool allowVariants = true)
	{
		if (_typeCache.TryGetValue(t, out HashSet<Type> value))
		{
			return value;
		}
		value = new HashSet<Type>(AllSteppedTypesInner(t, allowVariants));
		_typeCache[t] = value;
		return value;
	}

	private IEnumerable<Type> AllSteppedTypesInner(Type t, bool allowVariants)
	{
		Type type;
		do
		{
			yield return t;
			if (t == typeof(void))
			{
				break;
			}
			if (t.IsGenericType && allowVariants)
			{
				foreach (Type variant in t.GetVariants(this))
				{
					yield return variant;
				}
			}
			Type[] interfaces = t.GetInterfaces();
			foreach (Type t2 in interfaces)
			{
				foreach (Type item in AllSteppedTypes(t2, allowVariants))
				{
					yield return item;
				}
			}
			Type baseType = t.BaseType;
			if ((object)baseType != null)
			{
				foreach (Type item2 in AllSteppedTypes(baseType, allowVariants))
				{
					yield return item2;
				}
			}
			yield return typeof(IEnumerable<>).MakeGenericType(t);
			type = t;
			t = t.StepDownConstraints();
		}
		while (t != type);
	}

	internal bool IsTransformableTo(Type left, Type right)
	{
		if (left.IsAssignableToGeneric(right, this))
		{
			return true;
		}
		Type value = typeof(IAsType<>).MakeGenericType(right);
		if (left.GetInterfaces().Contains(value, null))
		{
			return true;
		}
		if (!right.IsGenericType(typeof(IEnumerable<>)))
		{
			return false;
		}
		return right.GenericTypeArguments[0] == left;
	}

	internal Expression GetTransformer(Type from, Type to, Expression input)
	{
		if (from.IsAssignableTo(to))
		{
			return Expression.Convert(input, to);
		}
		Type type = typeof(IAsType<>).MakeGenericType(to);
		if (from.GetInterfaces().Contains(type, null))
		{
			return Expression.Convert(Expression.Call(input, type.GetMethod("AsType")), to);
		}
		if (to.IsGenericType(typeof(IEnumerable<>)))
		{
			Type type2 = to.GenericTypeArguments[0];
			Type[] array = new Type[1] { type2 };
			return Expression.Convert(Expression.New(typeof(UnitEnumerable<>).MakeGenericType(array).GetConstructor(array), Expression.Convert(input, type2)), to);
		}
		if (from.IsAssignableToGeneric(to, this))
		{
			return Expression.Convert(input, to);
		}
		throw new InvalidCastException();
	}
}
