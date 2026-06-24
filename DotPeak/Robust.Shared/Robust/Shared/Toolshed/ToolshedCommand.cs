// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ToolshedCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Reflection;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
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

  public IEnumerable<string> Subcommands => (IEnumerable<string>) this.CommandImplementors.Keys;

  protected internal ToolshedCommand()
  {
  }

  internal void Init()
  {
    Type type1 = this.GetType();
    string str1 = type1.GetCustomAttribute<ToolshedCommandAttribute>().Name;
    if (str1 == null)
    {
      string name = type1.Name;
      string str2 = name.EndsWith("Command") ? name : throw new InvalidCommandImplementation($"Command {type1} must end with the word Command");
      int length = "Command".Length;
      str1 = str2.Substring(0, str2.Length - length).ToLowerInvariant();
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Name = !string.IsNullOrEmpty(str1) && str1.EnumerateRunes().All<Rune>(ToolshedCommand.\u003C\u003EO.\u003C0\u003E__IsCommandToken ?? (ToolshedCommand.\u003C\u003EO.\u003C0\u003E__IsCommandToken = new Func<Rune, bool>(ParserContext.IsCommandToken))) ? str1 : throw new InvalidCommandImplementation("Command name contains invalid tokens");
    foreach (Type typeParameterParser in this.TypeParameterParsers)
    {
      if (!(typeParameterParser == typeof (TypeTypeParser)) && !typeParameterParser.IsAssignableTo(typeof (CustomTypeParser<Type>)))
        throw new InvalidCommandImplementation($"{"TypeParameterParsers"} element {typeParameterParser} is not {"TypeTypeParser"} or assignable to {typeof (CustomTypeParser<Type>).PrettyName()}");
    }
    MethodInfo[] methods = this.GetMethods();
    if (methods.Length == 0)
      throw new Exception("Command has no implementations?");
    HashSet<(string, Type)> valueTupleSet = new HashSet<(string, Type)>();
    HashSet<string> stringSet = new HashSet<string>();
    bool flag1 = false;
    foreach (MethodInfo methodInfo in methods)
    {
      bool flag2 = false;
      bool flag3 = false;
      Type type2 = (Type) null;
      stringSet.Clear();
      foreach (ParameterInfo parameter in methodInfo.GetParameters())
      {
        bool flag4 = false;
        CommandArgumentAttribute customAttribute = parameter.GetCustomAttribute<CommandArgumentAttribute>();
        if (customAttribute != null)
        {
          if (parameter.Name == null || !stringSet.Add(parameter.Name))
            throw new InvalidCommandImplementation("Command arguments must have a unique name");
          flag4 = true;
          this.ValidateArg(parameter, customAttribute);
        }
        if (parameter.HasCustomAttribute<PipedArgumentAttribute>())
        {
          if (flag4)
            throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
          type2 = !(type2 != (Type) null) ? parameter.ParameterType : throw new InvalidCommandImplementation("Commands cannot have more than one piped argument");
          flag4 = true;
        }
        if (parameter.HasCustomAttribute<CommandInvertedAttribute>())
        {
          if (flag4)
            throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
          if (flag2)
            throw new InvalidCommandImplementation("Duplicate CommandInvertedAttribute");
          if (parameter.ParameterType != typeof (bool))
            throw new InvalidCommandImplementation("Command argument with the CommandInvertedAttribute must be of type bool");
          flag2 = true;
          flag4 = true;
        }
        if (parameter.HasCustomAttribute<CommandInvocationContextAttribute>())
        {
          if (flag4)
            throw new InvalidCommandImplementation("Method parameter cannot have more than one relevant attribute");
          if (flag3)
            throw new InvalidCommandImplementation("Duplicate CommandInvocationContextAttribute");
          if (parameter.ParameterType != typeof (IInvocationContext))
            throw new InvalidCommandImplementation("Command argument with the CommandInvocationContextAttribute must be of type IInvocationContext");
          flag3 = true;
          flag4 = true;
        }
        if (!flag4)
        {
          if (parameter.ParameterType == typeof (IInvocationContext))
          {
            flag3 = !flag3 ? true : throw new InvalidCommandImplementation("Duplicate (implicit?) CommandInvocationContextAttribute");
          }
          else
          {
            if (parameter.Name == null || !stringSet.Add(parameter.Name))
              throw new InvalidCommandImplementation("Command arguments must have a unique name");
            this.ValidateArg(parameter);
          }
        }
      }
      bool flag5 = methodInfo.HasCustomAttribute<TakesPipedTypeAsGenericAttribute>();
      int num = this.TypeParameterParsers.Length + (flag5 ? 1 : 0);
      if ((methodInfo.IsGenericMethodDefinition ? methodInfo.GetGenericArguments().Length : 0) != num)
        throw new InvalidCommandImplementation("Incorrect number of generic arguments.");
      if (flag5)
      {
        if (!methodInfo.IsGenericMethodDefinition)
          throw new InvalidCommandImplementation("TakesPipedTypeAsGenericAttribute requires a method to have generics");
        Type type3 = !(type2 == (Type) null) ? ToolshedCommandImplementor.GetGenericTypeFromPiped(type2, type2) : throw new InvalidCommandImplementation("TakesPipedTypeAsGenericAttribute required there to be a piped parameter");
        Type[] genericArguments = methodInfo.GetGenericArguments();
        Type type4 = genericArguments[genericArguments.Length - 1];
        if (type3 != type4)
          throw new InvalidCommandImplementation($"Commands using {"TakesPipedTypeAsGenericAttribute"} must have the inferred piped parameter type {type3.Name} be the last generic parameter");
      }
      string subCommand1 = (string) null;
      CommandImplementationAttribute customAttribute1 = methodInfo.GetCustomAttribute<CommandImplementationAttribute>();
      if (customAttribute1 != null)
      {
        string subCommand2 = customAttribute1.SubCommand;
        if (subCommand2 != null)
        {
          subCommand1 = subCommand2;
          this.HasSubCommands = true;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          if (string.IsNullOrEmpty(subCommand1) || !subCommand1.EnumerateRunes().All<Rune>(ToolshedCommand.\u003C\u003EO.\u003C1\u003E__IsToken ?? (ToolshedCommand.\u003C\u003EO.\u003C1\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken))))
            throw new InvalidCommandImplementation($"Subcommand name {subCommand1} contains invalid tokens");
          goto label_65;
        }
      }
      flag1 = true;
label_65:
      if (flag1 && this.HasSubCommands)
        throw new InvalidCommandImplementation("Toolshed commands either need to be all sub-commands, or have no sub commands at all.");
      if (!valueTupleSet.Add((subCommand1, type2)))
        throw new InvalidCommandImplementation("The combination of subcommand and piped parameter type must be unique");
      string key = subCommand1 ?? string.Empty;
      if (!this.CommandImplementors.ContainsKey(key))
        this.CommandImplementors[key] = new ToolshedCommandImplementor(subCommand1, this, this.Toolshed, this.Loc);
    }
  }

  private void ValidateArg(ParameterInfo arg, CommandArgumentAttribute? cmdAttr = null)
  {
    if (cmdAttr == null || cmdAttr.CustomParser == (Type) null && !cmdAttr.Unparseable)
    {
      Type type1 = Nullable.GetUnderlyingType(arg.ParameterType);
      if ((object) type1 == null)
        type1 = arg.ParameterType;
      Type type2 = type1;
      if ((type2.IsGenericType || type2.IsArray ? 1 : (type2.ContainsGenericParameters ? 1 : 0)) == 0 && this.Toolshed.GetParserForType(type2) == null)
        throw new InvalidCommandImplementation($"{this.Name} command argument of type {type2.PrettyName()} has no type parser. You either need to add a type parser or explicitly mark the argument as unparseable.");
    }
    if (arg.HasCustomAttribute<ParamArrayAttribute>() && !arg.ParameterType.IsArray)
      throw new InvalidCommandImplementation(".net 9 params collections are not yet supported");
  }

  internal HashSet<Type> AcceptedTypes(string? subCommand)
  {
    HashSet<Type> typeSet1;
    if (this._acceptedTypes.TryGetValue(subCommand ?? "", out typeSet1))
      return typeSet1;
    Dictionary<string, HashSet<Type>> acceptedTypes = this._acceptedTypes;
    string key = subCommand ?? "";
    IEnumerable<ParameterInfo> source = ((IEnumerable<MethodInfo>) this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<MethodInfo>((Func<MethodInfo, bool>) (x =>
    {
      CommandImplementationAttribute customAttribute = x.GetCustomAttribute<CommandImplementationAttribute>();
      return customAttribute != null && customAttribute.SubCommand == subCommand;
    })).Select<MethodInfo, ParameterInfo>((Func<MethodInfo, ParameterInfo>) (x => x.ConsoleGetPipedArgument())).Where<ParameterInfo>((Func<ParameterInfo, bool>) (x => x != null));
    HashSet<Type> hashSet;
    HashSet<Type> typeSet2 = hashSet = source.Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (x => x.ParameterType)).ToHashSet<Type>();
    acceptedTypes[key] = hashSet;
    return typeSet2;
  }

  protected EntityUid? ExecutingEntity(IInvocationContext ctx)
  {
    if (ctx.Session == null)
    {
      ctx.ReportError((IConError) new NotForServerConsoleError());
      return new EntityUid?();
    }
    EntityUid? attachedEntity = ctx.Session.AttachedEntity;
    if (attachedEntity.HasValue)
      return new EntityUid?(attachedEntity.GetValueOrDefault());
    ctx.ReportError((IConError) new SessionHasNoEntityError(ctx.Session));
    return new EntityUid?();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected MetaDataComponent MetaData(EntityUid entity)
  {
    return this.EntityManager.GetComponent<MetaDataComponent>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected TransformComponent Transform(EntityUid entity)
  {
    return this.EntityManager.GetComponent<TransformComponent>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected string EntName(EntityUid entity)
  {
    return this.EntityManager.GetComponent<MetaDataComponent>(entity).EntityName;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid Spawn(string? proto, EntityCoordinates coords)
  {
    return this.EntityManager.SpawnEntity(proto, coords);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid Spawn(string? proto, MapCoordinates coords)
  {
    return this.EntityManager.SpawnEntity(proto, coords);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Del(EntityUid entityUid)
  {
    this.EntityManager.DeleteEntity(new EntityUid?(entityUid));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void QDel(EntityUid entityUid)
  {
    this.EntityManager.QueueDeleteEntity(new EntityUid?(entityUid));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Deleted(EntityUid entity) => this.EntityManager.Deleted(entity);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T Comp<T>(EntityUid entity) where T : IComponent
  {
    return this.EntityManager.GetComponent<T>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool HasComp<T>(EntityUid entityUid) where T : IComponent
  {
    return this.EntityManager.HasComponent<T>(entityUid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp<T>([NotNullWhen(true)] EntityUid? entity, [NotNullWhen(true)] out T? component) where T : IComponent
  {
    return this.EntityManager.TryGetComponent<T>(entity, out component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp<T>(EntityUid entity, [NotNullWhen(true)] out T? component) where T : IComponent
  {
    return this.EntityManager.TryGetComponent<T>(entity, out component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T AddComp<T>(EntityUid entity) where T : IComponent, new()
  {
    return this.EntityManager.AddComponent<T>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void RemComp<T>(EntityUid entity) where T : IComponent
  {
    this.EntityManager.RemoveComponent<T>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T EnsureComp<T>(EntityUid entity) where T : IComponent, new()
  {
    return this.EntityManager.EnsureComponent<T>(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T GetSys<T>() where T : EntitySystem => this.EntitySystemManager.GetEntitySystem<T>();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T Sys<T>() where T : EntitySystem => this.EntitySystemManager.GetEntitySystem<T>();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityQuery<T> GetEntityQuery<T>() where T : IComponent
  {
    return this.EntityManager.GetEntityQuery<T>();
  }

  public string Description(string? subCommand)
  {
    ToolshedCommandImplementor commandImplementor;
    this.CommandImplementors.TryGetValue(subCommand ?? string.Empty, out commandImplementor);
    return commandImplementor?.Description() ?? string.Empty;
  }

  public string DescriptionLocKey(string? subCommand)
  {
    ToolshedCommandImplementor commandImplementor;
    this.CommandImplementors.TryGetValue(subCommand ?? string.Empty, out commandImplementor);
    return commandImplementor?.DescriptionLocKey() ?? string.Empty;
  }

  public string GetHelp(string? subCommand)
  {
    ToolshedCommandImplementor commandImplementor;
    this.CommandImplementors.TryGetValue(subCommand ?? string.Empty, out commandImplementor);
    return commandImplementor?.GetHelp() ?? string.Empty;
  }

  public override string ToString() => this.Name;

  public static string GetArgHint(CommandArgument? arg, Type t)
  {
    if (!arg.HasValue)
      return t.PrettyName();
    string name = arg.Value.Name;
    CommandArgument commandArgument = arg.Value;
    int num1 = commandArgument.IsOptional ? 1 : 0;
    commandArgument = arg.Value;
    int num2 = commandArgument.IsParamsCollection ? 1 : 0;
    Type t1 = t;
    return ToolshedCommand.GetArgHint(name, num1 != 0, num2 != 0, t1);
  }

  public static string GetArgHint(string name, bool optional, bool isParams, Type t)
  {
    string str = t.PrettyName();
    if (optional)
      return $"[{name} ({str})]";
    if (isParams)
      return $"[{name} ({str})]...";
    return $"<{name} ({str})>";
  }

  public bool TryGetReturnType(
    string? subCommand,
    Type? pipedType,
    Type[]? typeArguments,
    [NotNullWhen(true)] out Type? type)
  {
    type = (Type) null;
    ToolshedCommandImplementor commandImplementor;
    ConcreteCommandMethod? method;
    if (!this.CommandImplementors.TryGetValue(subCommand ?? string.Empty, out commandImplementor) || !commandImplementor.TryGetConcreteMethod(pipedType, typeArguments, out method))
      return false;
    type = method.Value.Info.ReturnType;
    return true;
  }

  internal MethodInfo[] GetMethods()
  {
    MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    return methods.Length != 1 ? ((IEnumerable<MethodInfo>) methods).Where<MethodInfo>((Func<MethodInfo, bool>) (x => x.HasCustomAttribute<CommandImplementationAttribute>())).ToArray<MethodInfo>() : methods;
  }

  internal MethodInfo[] GetMethods(string? subCommand)
  {
    return subCommand == null ? this.GetMethods() : ((IEnumerable<MethodInfo>) this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<MethodInfo>((Func<MethodInfo, bool>) (x => x.GetCustomAttribute<CommandImplementationAttribute>()?.SubCommand == subCommand)).ToArray<MethodInfo>();
  }
}
