// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ToolshedManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security;

#nullable enable
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
  private int _maxOutput = 128 /*0x80*/;
  private Dictionary<Type, HashSet<Type>> _typeCache = new Dictionary<Type, HashSet<Type>>();

  public void Initialize()
  {
    this._log = this._logManager.GetSawmill("toolshed");
    this.InitializeParser();
    this._player.PlayerStatusChanged += new EventHandler<SessionStatusEventArgs>(this.OnStatusChanged);
  }

  private void OnStatusChanged(object? sender, SessionStatusEventArgs e)
  {
    OldShellInvocationContext invocationContext;
    if (!this._contexts.TryGetValue(e.Session.UserId, out invocationContext))
      return;
    if (e.NewStatus == SessionStatus.Disconnected)
      invocationContext.Shell = (IConsoleShell) null;
    if (e.NewStatus != SessionStatus.InGame)
      return;
    invocationContext.Shell = (IConsoleShell) new ConsoleShell(this._conHost, e.Session, false);
  }

  public bool InvokeCommand(
    ICommonSession session,
    string command,
    object? input,
    out object? result)
  {
    OldShellInvocationContext ctx;
    if (!this._contexts.TryGetValue(session.UserId, out ctx))
    {
      ConsoleShell shell = new ConsoleShell(this._conHost, session, false);
      this._contexts[session.UserId] = ctx = new OldShellInvocationContext((IConsoleShell) shell);
    }
    ctx.ClearErrors();
    return this.InvokeCommand((IInvocationContext) ctx, command, input, out result);
  }

  public bool InvokeCommand(
    IConsoleShell session,
    string command,
    object? input,
    out object? result,
    out IInvocationContext ctx)
  {
    ICommonSession player = session.Player;
    NetUserId key = player != null ? player.UserId : new NetUserId();
    OldShellInvocationContext invocationContext;
    if (!this._contexts.TryGetValue(key, out invocationContext))
    {
      invocationContext = new OldShellInvocationContext(session);
      this._contexts[key] = invocationContext;
    }
    invocationContext.ClearErrors();
    ctx = (IInvocationContext) invocationContext;
    return this.InvokeCommand(ctx, command, input, out result);
  }

  public bool InvokeCommand(
    IInvocationContext ctx,
    string command,
    object? input,
    out object? result)
  {
    ctx.ClearErrors();
    result = (object) null;
    ParserContext ctx1 = new ParserContext(command, this, ctx);
    CommandRun expr;
    if (!CommandRun.TryParse(ctx1, input?.GetType(), (Type) null, out expr))
    {
      ctx.ReportError((IConError) ((object) ctx1.Error ?? (object) new FailedToParseError()));
      return false;
    }
    result = expr.Invoke(input, ctx);
    return true;
  }

  public CompletionResult? GetCompletions(ConsoleShell shell, string command)
  {
    ICommonSession player = shell.Player;
    NetUserId key = player != null ? player.UserId : new NetUserId();
    OldShellInvocationContext ctx;
    if (!this._contexts.TryGetValue(key, out ctx))
      ctx = this._contexts[key] = new OldShellInvocationContext((IConsoleShell) shell);
    return this.GetCompletions((IInvocationContext) ctx, command);
  }

  public CompletionResult? GetCompletions(IInvocationContext ctx, string command)
  {
    ctx.ClearErrors();
    ParserContext ctx1 = new ParserContext(command, this, ctx);
    ctx1.GenerateCompletions = true;
    CommandRun.TryParse(ctx1, (Type) null, (Type) null, out CommandRun _);
    return ctx1.Completions;
  }

  private void InitializeParser()
  {
    foreach (Type allChild in this._reflection.GetAllChildren<ITypeParser>())
    {
      Type baseType = allChild.BaseType;
      bool flag = false;
      for (; baseType != (Type) null; baseType = baseType.BaseType)
      {
        if (baseType.IsGenericType(typeof (TypeParser<>)))
        {
          flag = true;
          break;
        }
      }
      if (flag)
      {
        if (allChild.IsGenericType)
        {
          Type type = ((IEnumerable<Type>) allChild.BaseType.GetGenericArguments()).First<Type>();
          if (type.IsGenericType)
          {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (!this._genericTypeParsers.TryAdd(genericTypeDefinition, allChild))
              throw new Exception($"Duplicate toolshed type parser for type: {genericTypeDefinition}");
            this._log.Verbose($"Setting up {allChild.PrettyName()}, {type.GetGenericTypeDefinition().PrettyName()}");
          }
          else if (type.IsGenericParameter)
          {
            this._constrainedParsers.Add((type, allChild));
            this._log.Verbose($"Setting up {allChild.PrettyName()}, for T where T: {string.Join(", ", ((IEnumerable<Type>) type.GetGenericParameterConstraints()).Select<Type, string>((Func<Type, string>) (x => x.PrettyName())))}");
          }
        }
        else
        {
          ITypeParser instanceUnchecked = (ITypeParser) this._typeFactory.CreateInstanceUnchecked(allChild, true);
          if (instanceUnchecked is IPostInjectInit postInjectInit)
            postInjectInit.PostInject();
          this._log.Verbose($"Setting up {allChild.PrettyName()}, {instanceUnchecked.Parses.PrettyName()}");
          if (!this._consoleTypeParsers.TryAdd(instanceUnchecked.Parses, instanceUnchecked))
            throw new Exception($"Discovered conflicting parsers for type {instanceUnchecked.Parses.PrettyName()}: {allChild.PrettyName()} and {this._consoleTypeParsers[instanceUnchecked.Parses].GetType().PrettyName()}");
        }
      }
    }
  }

  internal ITypeParser? GetParserForType(Type t)
  {
    ITypeParser parserForType1;
    if (this._consoleTypeParsers.TryGetValue(t, out parserForType1))
      return parserForType1;
    ITypeParser parserForType2 = this.FindParserForType(t);
    this._consoleTypeParsers.TryAdd(t, parserForType2);
    return parserForType2;
  }

  internal ITypeParser? GetArgumentParser(Type t)
  {
    ITypeParser parserForType = this.GetParserForType(t);
    if (parserForType != null)
      return this.GetArgumentParser(parserForType);
    return this.GetParserForType(typeof (ValueRef<>).MakeGenericType(t));
  }

  internal ITypeParser? GetArgumentParser(ITypeParser baseParser)
  {
    if (!baseParser.EnableValueRef)
      return baseParser;
    ITypeParser argumentParser;
    if (this._argParsers.TryGetValue(baseParser, out argumentParser))
      return argumentParser;
    Type parses = baseParser.Parses;
    ITypeParser typeParser;
    if (parses.IsValueRef() || parses.IsAssignableTo(typeof (Block)))
      typeParser = baseParser;
    else if (baseParser.GetType().HasGenericParent(typeof (TypeParser<>)))
      typeParser = this.GetParserForType(typeof (ValueRef<>).MakeGenericType(parses));
    else
      typeParser = this.GetCustomParser(typeof (CustomValueRefTypeParser<,>).MakeGenericType(parses, baseParser.GetType()));
    return this._argParsers[baseParser] = typeParser;
  }

  internal TParser GetCustomParser<TParser, T>()
    where TParser : CustomTypeParser<T>, new()
    where T : notnull
  {
    return (TParser) this.GetCustomParser(typeof (TParser));
  }

  internal ITypeParser GetCustomParser(Type parser)
  {
    ITypeParser customParser;
    if (this._customParsers.TryGetValue(parser, out customParser))
      return customParser;
    if (parser.ContainsGenericParameters)
      throw new ArgumentException("Type cannot contain generic parameters");
    ITypeParser typeParser = parser.IsCustomParser() ? (ITypeParser) this._typeFactory.CreateInstanceUnchecked(parser, true) : throw new ArgumentException($"{parser.PrettyName()} does not inherit from {typeof (CustomTypeParser<>).PrettyName()}");
    if (typeParser is IPostInjectInit postInjectInit)
      postInjectInit.PostInject();
    return this._customParsers[parser] = typeParser;
  }

  private ITypeParser? FindParserForType(Type t)
  {
    if (t.IsConstructedGenericType)
    {
      Type type;
      if (this._genericTypeParsers.TryGetValue(t.GetGenericTypeDefinition(), out type))
      {
        try
        {
          ITypeParser instanceUnchecked = (ITypeParser) this._typeFactory.CreateInstanceUnchecked(type.MakeGenericType(t.GenericTypeArguments), true);
          if (instanceUnchecked is IPostInjectInit postInjectInit)
            postInjectInit.PostInject();
          return instanceUnchecked;
        }
        catch (SecurityException ex)
        {
          this._log.Info($"Couldn't use {type.PrettyName()} to parse {t.PrettyName()}");
        }
      }
    }
    foreach ((Type _, Type type) in this._constrainedParsers)
    {
      try
      {
        ITypeParser instanceUnchecked = (ITypeParser) this._typeFactory.CreateInstanceUnchecked(type.MakeGenericType(t), true);
        if (instanceUnchecked is IPostInjectInit postInjectInit)
          postInjectInit.PostInject();
        return instanceUnchecked;
      }
      catch (SecurityException ex)
      {
      }
      catch (ArgumentException ex)
      {
      }
    }
    Type baseType = t.BaseType;
    if ((object) baseType != null && baseType != typeof (object) && baseType != typeof (ValueType))
    {
      ITypeParser parserForType = this.GetParserForType(baseType);
      if (parserForType != null)
        return parserForType;
    }
    foreach (Type t1 in t.GetInterfaces())
    {
      ITypeParser parserForType = this.GetParserForType(t1);
      if (parserForType != null)
        return parserForType;
    }
    return (ITypeParser) null;
  }

  public bool TryParse<T>(ParserContext parserContext, [NotNullWhen(true)] out T? parsed)
  {
    Type nullableType = typeof (T);
    Type type = Nullable.GetUnderlyingType(nullableType);
    if ((object) type == null)
      type = nullableType;
    Type t = type;
    object parsed1;
    int num = this.TryParse(parserContext, t, out parsed1) ? 1 : 0;
    if (parsed1 != null)
    {
      parsed = (T) parsed1;
      return num != 0;
    }
    parsed = default (T);
    return num != 0;
  }

  public CompletionResult? TryAutocomplete(ParserContext ctx, Type t, CommandArgument? arg)
  {
    return this.GetParserForType(t)?.TryAutocomplete(ctx, arg);
  }

  public bool TryParse(ParserContext parserContext, Type t, [NotNullWhen(true)] out object? parsed)
  {
    parsed = (object) null;
    ITypeParser parserForType = this.GetParserForType(t);
    if (parserForType == null)
    {
      if (!parserContext.GenerateCompletions)
        parserContext.Error = (IConError) new UnparseableValueError(t);
      return false;
    }
    return parserForType.TryParse(parserContext, out parsed);
  }

  public bool TryParseArgument(ParserContext parserContext, Type t, [NotNullWhen(true)] out object? parsed)
  {
    parsed = (object) null;
    ITypeParser argumentParser = this.GetArgumentParser(t);
    return argumentParser != null && argumentParser.TryParse(parserContext, out parsed);
  }

  public IPermissionController? ActivePermissionController { get; set; }

  public bool CheckInvokable(CommandSpec command, ICommonSession? session, out IConError? error)
  {
    IPermissionController permissionController = this.ActivePermissionController;
    if (permissionController != null)
      return permissionController.CheckInvokable(command, session, out error);
    error = (IConError) null;
    return true;
  }

  public ToolshedEnvironment DefaultEnvironment
  {
    get
    {
      if (this._net.IsClient)
        throw new NotImplementedException("Toolshed is not yet ready for client-side use.");
      if (this._defaultEnvironment == null)
        this._defaultEnvironment = new ToolshedEnvironment();
      return this._defaultEnvironment;
    }
  }

  public string PrettyPrintType(object? value, out IEnumerable? more, bool moreUsed = false, int? maxOutput = null)
  {
    maxOutput.GetValueOrDefault();
    if (!maxOutput.HasValue)
      maxOutput = new int?(this._maxOutput);
    more = (IEnumerable) null;
    switch (value)
    {
      case null:
        return "";
      case IToolshedPrettyPrint toolshedPrettyPrint:
        return toolshedPrettyPrint.PrettyPrint(this, out more, moreUsed, maxOutput);
      case string str2:
        return str2.Length > 32768 /*0x8000*/ ? str2.Substring(0, 32768 /*0x8000*/) + "(refusing to output more!)" : str2;
      case FormattedMessage formattedMessage:
        return formattedMessage.ToMarkup();
      case EntityUid uid:
        return (string) this._entity.ToPrettyString((Entity<MetaDataComponent>) uid);
      default:
        Type type = value as Type;
        if ((object) type != null)
          return type.PrettyName();
        if (value.GetType().IsAssignableTo(typeof (IDictionary)))
        {
          IDictionaryEnumerator enumerator = ((IDictionary) value).GetEnumerator();
          List<string> values = new List<string>();
          while (enumerator.MoveNext())
          {
            IEnumerable more1;
            values.Add($"({this.PrettyPrintType(enumerator.Key, out more1)}, {this.PrettyPrintType(enumerator.Value, out more1)}");
          }
          return $"Dictionary {{\n{string.Join(",\n", (IEnumerable<string>) values)}\n}}";
        }
        if (!(value is IEnumerable source))
          return value.ToString() ?? "[unrepresentable]";
        List<object> list = source.Cast<object>().ToList<object>();
        if (list.Count > maxOutput.Value)
          more = (IEnumerable) list.GetRange(maxOutput.Value, list.Count - maxOutput.Value - 1);
        string str1 = string.Join(",\n", list.Take<object>(maxOutput.Value).Select<object, string>((Func<object, string>) (x => this.PrettyPrintType(x, out IEnumerable _))));
        if (more != null & moreUsed)
          return str1 + "... (output truncated, run more for further output)";
        return more != null ? str1 + "... (output truncated, if possible tee the value into it's own variable)" : str1;
    }
  }

  internal IEnumerable<Type> AllSteppedTypes(Type t, bool allowVariants = true)
  {
    HashSet<Type> typeSet1;
    if (this._typeCache.TryGetValue(t, out typeSet1))
      return (IEnumerable<Type>) typeSet1;
    HashSet<Type> typeSet2 = new HashSet<Type>(this.AllSteppedTypesInner(t, allowVariants));
    this._typeCache[t] = typeSet2;
    return (IEnumerable<Type>) typeSet2;
  }

  private IEnumerable<Type> AllSteppedTypesInner(Type t, bool allowVariants)
  {
    ToolshedManager toolshed = this;
    Type type;
    do
    {
      yield return t;
      if (t == typeof (void))
        break;
      if (t.IsGenericType & allowVariants)
      {
        foreach (Type variant in t.GetVariants(toolshed))
          yield return variant;
      }
      Type[] typeArray = t.GetInterfaces();
      for (int index = 0; index < typeArray.Length; ++index)
      {
        Type t1 = typeArray[index];
        foreach (Type allSteppedType in toolshed.AllSteppedTypes(t1, allowVariants))
          yield return allSteppedType;
      }
      typeArray = (Type[]) null;
      Type baseType = t.BaseType;
      if ((object) baseType != null)
      {
        foreach (Type allSteppedType in toolshed.AllSteppedTypes(baseType, allowVariants))
          yield return allSteppedType;
      }
      yield return typeof (IEnumerable<>).MakeGenericType(t);
      type = t;
      t = t.StepDownConstraints();
    }
    while (t != type);
  }

  internal bool IsTransformableTo(Type left, Type right)
  {
    if (left.IsAssignableToGeneric(right, this))
      return true;
    Type type = typeof (IAsType<>).MakeGenericType(right);
    if (((ReadOnlySpan<Type>) left.GetInterfaces()).Contains<Type>(type, (IEqualityComparer<Type>) null))
      return true;
    return right.IsGenericType(typeof (IEnumerable<>)) && right.GenericTypeArguments[0] == left;
  }

  internal Expression GetTransformer(Type from, Type to, Expression input)
  {
    if (from.IsAssignableTo(to))
      return (Expression) Expression.Convert(input, to);
    Type type = typeof (IAsType<>).MakeGenericType(to);
    if (((ReadOnlySpan<Type>) from.GetInterfaces()).Contains<Type>(type, (IEqualityComparer<Type>) null))
      return (Expression) Expression.Convert((Expression) Expression.Call(input, type.GetMethod("AsType")), to);
    if (to.IsGenericType(typeof (IEnumerable<>)))
    {
      Type genericTypeArgument = to.GenericTypeArguments[0];
      Type[] types = new Type[1]{ genericTypeArgument };
      return (Expression) Expression.Convert((Expression) Expression.New(typeof (UnitEnumerable<>).MakeGenericType(types).GetConstructor(types), (Expression) Expression.Convert(input, genericTypeArgument)), to);
    }
    return from.IsAssignableToGeneric(to, this) ? (Expression) Expression.Convert(input, to) : throw new InvalidCastException();
  }
}
