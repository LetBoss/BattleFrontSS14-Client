using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Reflection;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

[Reflect(false)]
public abstract class ToolshedCommand
{
	[Robust.Shared.IoC.Dependency]
	protected readonly ToolshedManager Toolshed;

	[Robust.Shared.IoC.Dependency]
	protected readonly ILocalizationManager Loc;

	public bool HasSubCommands;

	internal readonly Dictionary<string, ToolshedCommandImplementor> CommandImplementors = new Dictionary<string, ToolshedCommandImplementor>();

	private readonly Dictionary<string, HashSet<Type>> _acceptedTypes = new Dictionary<string, HashSet<Type>>();

	[Robust.Shared.IoC.Dependency]
	protected readonly IEntityManager EntityManager;

	[Robust.Shared.IoC.Dependency]
	protected readonly IEntitySystemManager EntitySystemManager;

	public const BindingFlags MethodFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public string Name { get; private set; }

	public virtual Type[] TypeParameterParsers => Array.Empty<Type>();

	public IEnumerable<string> Subcommands => CommandImplementors.Keys;

	protected internal ToolshedCommand()
	{
	}

	internal void Init()
	{
		Type type = GetType();
		string text = type.GetCustomAttribute<ToolshedCommandAttribute>().Name;
		if (text == null)
		{
			string name = type.Name;
			if (!name.EndsWith("Command"))
			{
				throw new InvalidCommandImplementation($"Command {type} must end with the word Command");
			}
			string text2 = name;
			int length = "Command".Length;
			text = text2.Substring(0, text2.Length - length).ToLowerInvariant();
		}
		if (string.IsNullOrEmpty(text) || !text.EnumerateRunes().All(ParserContext.IsCommandToken))
		{
			throw new InvalidCommandImplementation("Command name contains invalid tokens");
		}
		Name = text;
		Type[] typeParameterParsers = TypeParameterParsers;
		foreach (Type type2 in typeParameterParsers)
		{
			if (!(type2 == typeof(TypeTypeParser)) && !type2.IsAssignableTo(typeof(CustomTypeParser<Type>)))
			{
				throw new InvalidCommandImplementation($"{"TypeParameterParsers"} element {type2} is not {"TypeTypeParser"} or assignable to {typeof(CustomTypeParser<Type>).PrettyName()}");
			}
		}
		MethodInfo[] methods = GetMethods();
		if (methods.Length == 0)
		{
			throw new Exception("Command has no implementations?");
		}
		HashSet<(string, Type)> hashSet = new HashSet<(string, Type)>();
		HashSet<string> hashSet2 = new HashSet<string>();
		bool flag = false;
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			bool flag2 = false;
			bool flag3 = false;
			Type type3 = null;
			hashSet2.Clear();
			ParameterInfo[] parameters = methodInfo.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				bool flag4 = false;
				CommandArgumentAttribute customAttribute = parameterInfo.GetCustomAttribute<CommandArgumentAttribute>();
				if (customAttribute != null)
				{
					if (parameterInfo.Name == null || !hashSet2.Add(parameterInfo.Name))
					{
						throw new InvalidCommandImplementation("Command arguments must have a unique name");
					}
					flag4 = true;
					ValidateArg(parameterInfo, customAttribute);
				}
				if (parameterInfo.HasCustomAttribute<PipedArgumentAttribute>())
				{
					if (flag4)
					{
						throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
					}
					if (type3 != null)
					{
						throw new InvalidCommandImplementation("Commands cannot have more than one piped argument");
					}
					type3 = parameterInfo.ParameterType;
					flag4 = true;
				}
				if (parameterInfo.HasCustomAttribute<CommandInvertedAttribute>())
				{
					if (flag4)
					{
						throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
					}
					if (flag2)
					{
						throw new InvalidCommandImplementation("Duplicate CommandInvertedAttribute");
					}
					if (parameterInfo.ParameterType != typeof(bool))
					{
						throw new InvalidCommandImplementation("Command argument with the CommandInvertedAttribute must be of type bool");
					}
					flag2 = true;
					flag4 = true;
				}
				if (parameterInfo.HasCustomAttribute<CommandInvocationContextAttribute>())
				{
					if (flag4)
					{
						throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
					}
					if (flag3)
					{
						throw new InvalidCommandImplementation("Duplicate CommandInvocationContextAttribute");
					}
					if (parameterInfo.ParameterType != typeof(IInvocationContext))
					{
						throw new InvalidCommandImplementation("Command argument with the CommandInvocationContextAttribute must be of type IInvocationContext");
					}
					flag3 = true;
					flag4 = true;
				}
				if (flag4)
				{
					continue;
				}
				if (parameterInfo.ParameterType == typeof(IInvocationContext))
				{
					if (flag3)
					{
						throw new InvalidCommandImplementation("Duplicate (implicit?) CommandInvocationContextAttribute");
					}
					flag3 = true;
				}
				else
				{
					if (parameterInfo.Name == null || !hashSet2.Add(parameterInfo.Name))
					{
						throw new InvalidCommandImplementation("Command arguments must have a unique name");
					}
					ValidateArg(parameterInfo);
				}
			}
			bool flag5 = methodInfo.HasCustomAttribute<TakesPipedTypeAsGenericAttribute>();
			int num = TypeParameterParsers.Length + (flag5 ? 1 : 0);
			if ((methodInfo.IsGenericMethodDefinition ? methodInfo.GetGenericArguments().Length : 0) != num)
			{
				throw new InvalidCommandImplementation("Incorrect number of generic arguments.");
			}
			if (flag5)
			{
				if (!methodInfo.IsGenericMethodDefinition)
				{
					throw new InvalidCommandImplementation("TakesPipedTypeAsGenericAttribute requires a method to have generics");
				}
				if (type3 == null)
				{
					throw new InvalidCommandImplementation("TakesPipedTypeAsGenericAttribute required there to be a piped parameter");
				}
				Type genericTypeFromPiped = ToolshedCommandImplementor.GetGenericTypeFromPiped(type3, type3);
				Type type4 = methodInfo.GetGenericArguments()[^1];
				if (genericTypeFromPiped != type4)
				{
					throw new InvalidCommandImplementation($"Commands using {"TakesPipedTypeAsGenericAttribute"} must have the inferred piped parameter type {genericTypeFromPiped.Name} be the last generic parameter");
				}
			}
			string text3 = null;
			CommandImplementationAttribute customAttribute2 = methodInfo.GetCustomAttribute<CommandImplementationAttribute>();
			if (customAttribute2 != null)
			{
				string subCommand = customAttribute2.SubCommand;
				if (subCommand != null)
				{
					text3 = subCommand;
					HasSubCommands = true;
					if (string.IsNullOrEmpty(text3) || !text3.EnumerateRunes().All(ParserContext.IsToken))
					{
						throw new InvalidCommandImplementation("Subcommand name " + text3 + " contains invalid tokens");
					}
					goto IL_04d2;
				}
			}
			flag = true;
			goto IL_04d2;
			IL_04d2:
			if (flag && HasSubCommands)
			{
				throw new InvalidCommandImplementation("Toolshed commands either need to be all sub-commands, or have no sub commands at all.");
			}
			if (!hashSet.Add((text3, type3)))
			{
				throw new InvalidCommandImplementation("The combination of subcommand and piped parameter type must be unique");
			}
			string key = text3 ?? string.Empty;
			if (!CommandImplementors.ContainsKey(key))
			{
				CommandImplementors[key] = new ToolshedCommandImplementor(text3, this, Toolshed, Loc);
			}
		}
	}

	private void ValidateArg(ParameterInfo arg, CommandArgumentAttribute? cmdAttr = null)
	{
		if (cmdAttr == null || (cmdAttr.CustomParser == null && !cmdAttr.Unparseable))
		{
			Type type = Nullable.GetUnderlyingType(arg.ParameterType) ?? arg.ParameterType;
			if (!type.IsGenericType && !type.IsArray && !type.ContainsGenericParameters && Toolshed.GetParserForType(type) == null)
			{
				throw new InvalidCommandImplementation(Name + " command argument of type " + type.PrettyName() + " has no type parser. You either need to add a type parser or explicitly mark the argument as unparseable.");
			}
		}
		if (!arg.HasCustomAttribute<ParamArrayAttribute>() || arg.ParameterType.IsArray)
		{
			return;
		}
		throw new InvalidCommandImplementation(".net 9 params collections are not yet supported");
	}

	internal HashSet<Type> AcceptedTypes(string? subCommand)
	{
		if (_acceptedTypes.TryGetValue(subCommand ?? "", out HashSet<Type> value))
		{
			return value;
		}
		return _acceptedTypes[subCommand ?? ""] = (from x in GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(delegate(MethodInfo x)
			{
				CommandImplementationAttribute customAttribute = x.GetCustomAttribute<CommandImplementationAttribute>();
				return customAttribute != null && customAttribute.SubCommand == subCommand;
			})
			select x.ConsoleGetPipedArgument() into x
			where x != null
			select x.ParameterType).ToHashSet();
	}

	protected EntityUid? ExecutingEntity(IInvocationContext ctx)
	{
		if (ctx.Session == null)
		{
			ctx.ReportError(new NotForServerConsoleError());
			return null;
		}
		EntityUid? attachedEntity = ctx.Session.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			return attachedEntity.GetValueOrDefault();
		}
		ctx.ReportError(new SessionHasNoEntityError(ctx.Session));
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected MetaDataComponent MetaData(EntityUid entity)
	{
		return EntityManager.GetComponent<MetaDataComponent>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected TransformComponent Transform(EntityUid entity)
	{
		return EntityManager.GetComponent<TransformComponent>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected string EntName(EntityUid entity)
	{
		return EntityManager.GetComponent<MetaDataComponent>(entity).EntityName;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid Spawn(string? proto, EntityCoordinates coords)
	{
		return EntityManager.SpawnEntity(proto, coords);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid Spawn(string? proto, MapCoordinates coords)
	{
		return EntityManager.SpawnEntity(proto, coords);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Del(EntityUid entityUid)
	{
		EntityManager.DeleteEntity(entityUid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void QDel(EntityUid entityUid)
	{
		EntityManager.QueueDeleteEntity(entityUid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Deleted(EntityUid entity)
	{
		return EntityManager.Deleted(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T Comp<T>(EntityUid entity) where T : IComponent
	{
		return EntityManager.GetComponent<T>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool HasComp<T>(EntityUid entityUid) where T : IComponent
	{
		return EntityManager.HasComponent<T>(entityUid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryComp<T>([NotNullWhen(true)] EntityUid? entity, [NotNullWhen(true)] out T? component) where T : IComponent
	{
		return EntityManager.TryGetComponent<T>(entity, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryComp<T>(EntityUid entity, [NotNullWhen(true)] out T? component) where T : IComponent
	{
		return EntityManager.TryGetComponent<T>(entity, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T AddComp<T>(EntityUid entity) where T : IComponent, new()
	{
		return EntityManager.AddComponent<T>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RemComp<T>(EntityUid entity) where T : IComponent
	{
		EntityManager.RemoveComponent<T>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T EnsureComp<T>(EntityUid entity) where T : IComponent, new()
	{
		return EntityManager.EnsureComponent<T>(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T GetSys<T>() where T : EntitySystem
	{
		return EntitySystemManager.GetEntitySystem<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T Sys<T>() where T : EntitySystem
	{
		return EntitySystemManager.GetEntitySystem<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQuery<T> GetEntityQuery<T>() where T : IComponent
	{
		return EntityManager.GetEntityQuery<T>();
	}

	public string Description(string? subCommand)
	{
		CommandImplementors.TryGetValue(subCommand ?? string.Empty, out ToolshedCommandImplementor value);
		return value?.Description() ?? string.Empty;
	}

	public string DescriptionLocKey(string? subCommand)
	{
		CommandImplementors.TryGetValue(subCommand ?? string.Empty, out ToolshedCommandImplementor value);
		return value?.DescriptionLocKey() ?? string.Empty;
	}

	public string GetHelp(string? subCommand)
	{
		CommandImplementors.TryGetValue(subCommand ?? string.Empty, out ToolshedCommandImplementor value);
		return value?.GetHelp() ?? string.Empty;
	}

	public override string ToString()
	{
		return Name;
	}

	public static string GetArgHint(CommandArgument? arg, Type t)
	{
		if (!arg.HasValue)
		{
			return t.PrettyName();
		}
		return GetArgHint(arg.Value.Name, arg.Value.IsOptional, arg.Value.IsParamsCollection, t);
	}

	public static string GetArgHint(string name, bool optional, bool isParams, Type t)
	{
		string value = t.PrettyName();
		if (!optional)
		{
			if (!isParams)
			{
				return $"<{name} ({value})>";
			}
			return $"[{name} ({value})]...";
		}
		return $"[{name} ({value})]";
	}

	public bool TryGetReturnType(string? subCommand, Type? pipedType, Type[]? typeArguments, [NotNullWhen(true)] out Type? type)
	{
		type = null;
		if (!CommandImplementors.TryGetValue(subCommand ?? string.Empty, out ToolshedCommandImplementor value))
		{
			return false;
		}
		if (!value.TryGetConcreteMethod(pipedType, typeArguments, out var method))
		{
			return false;
		}
		type = method.Value.Info.ReturnType;
		return true;
	}

	internal MethodInfo[] GetMethods()
	{
		MethodInfo[] methods = GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (methods.Length != 1)
		{
			return methods.Where((MethodInfo x) => x.HasCustomAttribute<CommandImplementationAttribute>()).ToArray();
		}
		return methods;
	}

	internal MethodInfo[] GetMethods(string? subCommand)
	{
		if (subCommand == null)
		{
			return GetMethods();
		}
		return (from x in GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where x.GetCustomAttribute<CommandImplementationAttribute>()?.SubCommand == subCommand
			select x).ToArray();
	}
}
