// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ToolshedCommandImplementor
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Exceptions;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed;

internal sealed class ToolshedCommandImplementor
{
  public readonly ToolshedCommand Owner;
  public readonly string? SubCommand;
  public readonly string FullName;
  public readonly string LocName;
  private readonly ToolshedManager _toolshed;
  private readonly ILocalizationManager _loc;
  public readonly Dictionary<CommandDiscriminator, Func<CommandInvocationArguments, object?>> Implementations = new Dictionary<CommandDiscriminator, Func<CommandInvocationArguments, object>>();
  private readonly Dictionary<CommandDiscriminator, ConcreteCommandMethod?> _methodCache = new Dictionary<CommandDiscriminator, ConcreteCommandMethod?>();
  internal readonly CommandMethod[] Methods;

  public CommandSpec Spec => new CommandSpec(this.Owner, this.SubCommand);

  public ToolshedCommandImplementor(
    string? subCommand,
    ToolshedCommand owner,
    ToolshedManager toolshed,
    ILocalizationManager loc)
  {
    this.Owner = owner;
    this._loc = loc;
    this.SubCommand = subCommand;
    this.FullName = this.SubCommand == null ? this.Owner.Name : $"{this.Owner.Name}:{this.SubCommand}";
    this._toolshed = toolshed;
    this.Methods = ((IEnumerable<MethodInfo>) this.Owner.GetMethods(this.SubCommand)).Select<MethodInfo, CommandMethod>((Func<MethodInfo, CommandMethod>) (x => new CommandMethod(x, this))).ToArray<CommandMethod>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.LocName = this.Owner.Name.All<char>(ToolshedCommandImplementor.\u003C\u003EO.\u003C0\u003E__IsAsciiLetterOrDigit ?? (ToolshedCommandImplementor.\u003C\u003EO.\u003C0\u003E__IsAsciiLetterOrDigit = new Func<char, bool>(char.IsAsciiLetterOrDigit))) ? this.Owner.Name : this.Owner.GetType().PrettyName();
    if (this.SubCommand == null)
      return;
    this.LocName = $"{this.LocName}-{this.SubCommand}";
  }

  public bool TryParse(
    ParserContext ctx,
    out Func<CommandInvocationArguments, object?>? invocable,
    [NotNullWhen(true)] out ConcreteCommandMethod? method)
  {
    ctx.ConsumeWhitespace();
    method = new ConcreteCommandMethod?();
    invocable = (Func<CommandInvocationArguments, object>) null;
    if (!this.TryParseTypeArguments(ctx))
      return false;
    if (!this.TryGetConcreteMethod(ctx.Bundle.PipedType, ctx.Bundle.TypeArguments, out method))
    {
      if (ctx.GenerateCompletions)
        return false;
      ctx.Error = (IConError) new NoImplementationError(ctx);
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Bundle.NameStart, ctx.Bundle.NameEnd)));
      return false;
    }
    int index = ctx.Index;
    if (!this.TryParseArguments(ctx, method.Value))
    {
      ctx.Error?.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    invocable = this.GetImplementation(ctx.Bundle, method.Value);
    return true;
  }

  public bool TryParseArguments(ParserContext ctx, ConcreteCommandMethod method)
  {
    foreach (CommandArgument commandArgument in method.Args)
    {
      object obj;
      if (commandArgument.IsParamsCollection)
      {
        if (!this.ParseParamsCollection(ctx, commandArgument, out obj))
          return false;
      }
      else if (!this.TryParseArgument(ctx, commandArgument, out obj))
        return false;
      ref Dictionary<string, object> local = ref ctx.Bundle.Arguments;
      if (local == null)
        local = new Dictionary<string, object>();
      ctx.Bundle.Arguments[commandArgument.Name] = obj;
    }
    return true;
  }

  private bool ParseParamsCollection(ParserContext ctx, CommandArgument arg, out object? collection)
  {
    List<object> objectList = new List<object>();
    collection = (object) objectList;
    while (true)
    {
      ctx.ConsumeWhitespace();
      if (!ctx.PeekCommandOrBlockTerminated() && (ctx == null || !ctx.OutOfInput || ctx.GenerateCompletions))
      {
        object parsed;
        if (this.TryParseArgument(ctx, arg, out parsed))
          objectList.Add(parsed);
        else
          break;
      }
      else
        goto label_5;
    }
    return false;
label_5:
    return true;
  }

  private bool TryParseArgument(ParserContext ctx, CommandArgument arg, out object? parsed)
  {
    int index = ctx.Index;
    ParserRestorePoint point = ctx.Save();
    ctx.ConsumeWhitespace();
    parsed = (object) null;
    if (ctx.PeekCommandOrBlockTerminated() || ctx != null && ctx.OutOfInput && !ctx.GenerateCompletions)
    {
      if (!arg.IsOptional)
      {
        ctx.Error = (IConError) new ExpectedArgumentError(arg.Type);
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
        return false;
      }
      parsed = arg.DefaultValue;
    }
    else if (!arg.Parser.TryParse(ctx, out parsed))
    {
      if (ctx.GenerateCompletions)
      {
        if (!ctx.OutOfInput || ctx.Completions != (CompletionResult) null)
          return false;
        ctx.Restore(point);
        ctx.Error = (IConError) null;
        ParserContext parserContext = ctx;
        if ((object) parserContext.Completions == null)
          parserContext.Completions = arg.Parser.TryAutocomplete(ctx, new CommandArgument?(arg));
        this.TrySetArgHint(ctx, arg);
        return false;
      }
      int num = Math.Max(index + 1, ctx.Index);
      ParserContext parserContext1 = ctx;
      if (parserContext1.Error == null)
        parserContext1.Error = (IConError) new ArgumentParseError(arg.Type, arg.Parser.GetType());
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, num)));
      return false;
    }
    if (!ctx.GenerateCompletions || !ctx.OutOfInput)
      return true;
    ctx.Restore(point);
    ctx.Error = (IConError) null;
    ParserContext parserContext2 = ctx;
    if ((object) parserContext2.Completions == null)
      parserContext2.Completions = arg.Parser.TryAutocomplete(ctx, new CommandArgument?(arg));
    this.TrySetArgHint(ctx, arg);
    return false;
  }

  private void TrySetArgHint(ParserContext ctx, CommandArgument arg)
  {
    string str;
    if (ctx.Completions == (CompletionResult) null || !this._loc.TryGetString($"command-arg-hint-{this.LocName}-{arg.Name}", out str))
      return;
    ctx.Completions.Hint = str;
  }

  internal bool TryParseTypeArguments(ParserContext ctx)
  {
    if (this.Owner.TypeParameterParsers.Length == 0)
      return true;
    ref Type[] local = ref ctx.Bundle.TypeArguments;
    local = new Type[this.Owner.TypeParameterParsers.Length];
    for (int index1 = 0; index1 < this.Owner.TypeParameterParsers.Length; ++index1)
    {
      Type typeParameterParser = this.Owner.TypeParameterParsers[index1];
      int index2 = ctx.Index;
      ctx.ConsumeWhitespace();
      ParserRestorePoint point = ctx.Save();
      if (ctx != null && ctx.OutOfInput && !ctx.GenerateCompletions || ctx.PeekCommandOrBlockTerminated())
      {
        ctx.Error = (IConError) new ExpectedTypeArgumentError();
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index2, ctx.Index + 1)));
        return false;
      }
      BaseParser<Type> baseParser = typeParameterParser == typeof (TypeTypeParser) ? (BaseParser<Type>) this._toolshed.GetParserForType(typeof (Type)) : (BaseParser<Type>) this._toolshed.GetCustomParser(typeParameterParser);
      Type result;
      if (!baseParser.TryParse(ctx, out result))
      {
        local = (Type[]) null;
        if (ctx.GenerateCompletions)
        {
          if (!ctx.OutOfInput)
            return false;
          ctx.Restore(point);
          ctx.Error = (IConError) null;
          ParserContext parserContext = ctx;
          if ((object) parserContext.Completions == null)
            parserContext.Completions = baseParser.TryAutocomplete(ctx, new CommandArgument?());
          return false;
        }
        ParserContext parserContext1 = ctx;
        if (parserContext1.Error == null)
          parserContext1.Error = (IConError) new TypeArgumentParseError(typeParameterParser);
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index2, ctx.Index)));
        return false;
      }
      local[index1] = result;
      if (ctx.GenerateCompletions && ctx.OutOfInput)
      {
        ctx.Restore(point);
        ctx.Error = (IConError) null;
        ctx.Completions = baseParser.TryAutocomplete(ctx, new CommandArgument?());
        return false;
      }
    }
    return true;
  }

  internal bool TryGetConcreteMethod(
    Type? pipedType,
    Type[]? typeArguments,
    [NotNullWhen(true)] out ConcreteCommandMethod? method)
  {
    CommandDiscriminator key1 = new CommandDiscriminator(pipedType, typeArguments);
    if (this._methodCache.TryGetValue(key1, out method))
      return method.HasValue;
    (CommandMethod, MethodInfo)? concreteMethodInternal = this.GetConcreteMethodInternal(pipedType, typeArguments);
    if (!concreteMethodInternal.HasValue)
    {
      Dictionary<CommandDiscriminator, ConcreteCommandMethod?> methodCache = this._methodCache;
      CommandDiscriminator key2 = key1;
      ref ConcreteCommandMethod? local = ref method;
      ConcreteCommandMethod? nullable1 = new ConcreteCommandMethod?();
      ConcreteCommandMethod? nullable2;
      ConcreteCommandMethod? nullable3 = nullable2 = nullable1;
      local = nullable2;
      ConcreteCommandMethod? nullable4 = nullable3;
      methodCache[key2] = nullable4;
      return false;
    }
    (CommandMethod Base, MethodInfo Info) = concreteMethodInternal.Value;
    if ((object) pipedType != null && pipedType.ContainsGenericParameters || typeArguments != null && ((IEnumerable<Type>) typeArguments).Any<Type>((Func<Type, bool>) (x => x.ContainsGenericParameters)))
    {
      this._methodCache[key1] = method = new ConcreteCommandMethod?(new ConcreteCommandMethod(Info, (CommandArgument[]) null, Base));
      return true;
    }
    CommandArgument[] array = ((IEnumerable<ParameterInfo>) Info.GetParameters()).Where<ParameterInfo>((Func<ParameterInfo, bool>) (x => x.IsCommandArgument())).Select<ParameterInfo, CommandArgument>(new Func<ParameterInfo, CommandArgument>(this.GetCommandArgument)).ToArray<CommandArgument>();
    this._methodCache[key1] = method = new ConcreteCommandMethod?(new ConcreteCommandMethod(Info, array, Base));
    return true;
  }

  internal CommandArgument GetCommandArgument(ParameterInfo arg)
  {
    Type type = arg.ParameterType;
    bool IsParamsCollection = arg.HasCustomAttribute<ParamArrayAttribute>();
    if (IsParamsCollection)
      type = type.IsArray ? type.GetElementType() : throw new NotSupportedException(".net 9 params collections are not yet supported");
    return new CommandArgument(arg.Name, type, this.GetArgumentParser(arg, type), arg.IsOptional, arg.DefaultValue, IsParamsCollection);
  }

  private ITypeParser? GetArgumentParser(ParameterInfo param, Type type)
  {
    if (type.ContainsGenericParameters)
      return (ITypeParser) null;
    Type customParser = param.GetCustomAttribute<CommandArgumentAttribute>()?.CustomParser;
    return ((object) customParser == null ? this._toolshed.GetArgumentParser(type) : this._toolshed.GetArgumentParser(this._toolshed.GetCustomParser(customParser))) ?? throw new Exception($"No parser for type: {param.ParameterType}");
  }

  private (CommandMethod, MethodInfo)? GetConcreteMethodInternal(
    Type? pipedType,
    Type[]? typeArguments)
  {
    return ((IEnumerable<CommandMethod>) this.Methods).Where<CommandMethod>((Func<CommandMethod, bool>) (x =>
    {
      ParameterInfo pipeArg = x.PipeArg;
      if (pipeArg == null)
        return (object) pipedType == null;
      if (pipedType == (Type) null)
        return false;
      return x.Generic || this._toolshed.IsTransformableTo(pipedType, pipeArg.ParameterType);
    })).OrderByDescending<CommandMethod, int>((Func<CommandMethod, int>) (x =>
    {
      ParameterInfo pipeArg = x.PipeArg;
      return pipeArg == null ? 0 : this.GetMethodRating(pipedType, pipeArg.ParameterType);
    })).Select<CommandMethod, (CommandMethod, MethodInfo)?>((Func<CommandMethod, (CommandMethod, MethodInfo)?>) (x =>
    {
      if (!x.Generic)
        return new (CommandMethod, MethodInfo)?((x, x.Info));
      try
      {
        if (!x.PipeGeneric)
          return new (CommandMethod, MethodInfo)?((x, x.Info.MakeGenericMethod(typeArguments)));
        Type genericTypeFromPiped = ToolshedCommandImplementor.GetGenericTypeFromPiped(pipedType, x.PipeArg.ParameterType);
        CommandMethod commandMethod = x;
        MethodInfo info = x.Info;
        Type[] source = typeArguments;
        Type[] typeArray = source != null ? ((IEnumerable<Type>) source).Append<Type>(genericTypeFromPiped).ToArray<Type>() : (Type[]) null;
        if (typeArray == null)
          typeArray = new Type[1]{ genericTypeFromPiped };
        MethodInfo methodInfo = info.MakeGenericMethod(typeArray);
        return new (CommandMethod, MethodInfo)?((commandMethod, methodInfo));
      }
      catch (ArgumentException ex)
      {
        return new (CommandMethod, MethodInfo)?();
      }
    })).FirstOrDefault<(CommandMethod, MethodInfo)?>((Func<(CommandMethod, MethodInfo)?, bool>) (x => x.HasValue));
  }

  private int GetMethodRating(Type? pipedType, Type paramType)
  {
    if (pipedType.IsAssignableTo(paramType))
      return 1000;
    if (!paramType.ContainsGenericParameters)
      return paramType.GetMostGenericPossible() == pipedType.GetMostGenericPossible() ? 500 : 400;
    if (paramType.GetMostGenericPossible() == pipedType.GetMostGenericPossible())
      return 300;
    return paramType.IsGenericParameter ? Math.Min(100 + paramType.GetGenericParameterConstraints().Length, 299) : 0;
  }

  public static Type GetGenericTypeFromPiped(Type inputType, Type parameterType)
  {
    return inputType.Intersect(parameterType);
  }

  public Func<CommandInvocationArguments, object?> GetImplementation(
    CommandArgumentBundle args,
    ConcreteCommandMethod method)
  {
    CommandDiscriminator key = new CommandDiscriminator(args.PipedType, args.TypeArguments);
    Func<CommandInvocationArguments, object> implementationInternal;
    if (!this.Implementations.TryGetValue(key, out implementationInternal))
      this.Implementations[key] = implementationInternal = this.GetImplementationInternal(args, method);
    return implementationInternal;
  }

  internal Func<CommandInvocationArguments, object?> GetImplementationInternal(
    CommandArgumentBundle cmdArgs,
    ConcreteCommandMethod method)
  {
    ParameterExpression args = Expression.Parameter(typeof (CommandInvocationArguments));
    List<Expression> arguments = new List<Expression>();
    foreach (ParameterInfo parameter in method.Info.GetParameters())
      arguments.Add(this.GetParamExpr(parameter, cmdArgs.PipedType, args));
    Expression expression = (Expression) Expression.Call((Expression) Expression.Constant((object) this.Owner), method.Info, (IEnumerable<Expression>) arguments);
    Type returnType = method.Info.ReturnType;
    if (returnType == typeof (void))
      expression = (Expression) Expression.Block(expression, (Expression) Expression.Constant((object) null));
    else if (returnType.IsValueType)
      expression = (Expression) Expression.Convert(expression, typeof (object));
    return Expression.Lambda<Func<CommandInvocationArguments, object>>(expression, args).Compile();
  }

  private Expression GetParamExpr(ParameterInfo param, Type? pipedType, ParameterExpression args)
  {
    if (param.HasCustomAttribute<PipedArgumentAttribute>())
    {
      if ((object) pipedType == null)
        throw new TypeArgumentException();
      return this._toolshed.GetTransformer(pipedType, param.ParameterType, (Expression) Expression.Field((Expression) args, "PipedArgument"));
    }
    if (param.HasCustomAttribute<CommandInvertedAttribute>())
      return (Expression) Expression.Property((Expression) args, "Inverted");
    return param.HasCustomAttribute<CommandArgumentAttribute>() || !param.HasCustomAttribute<CommandInvocationContextAttribute>() && !(param.ParameterType == typeof (IInvocationContext)) ? this.GetArgExpr(param, args) : (Expression) Expression.Property((Expression) args, "Context");
  }

  private Expression GetArgExpr(ParameterInfo param, ParameterExpression args)
  {
    IndexExpression indexExpression = Expression.MakeIndex((Expression) Expression.Property((Expression) args, "Arguments"), typeof (Dictionary<string, object>).FindIndexerProperty(), (IEnumerable<Expression>) new ConstantExpression[1]
    {
      Expression.Constant((object) param.Name)
    });
    MemberExpression memberExpression = Expression.Property((Expression) args, "Context");
    MethodInfo method;
    if (param.HasCustomAttribute<ParamArrayAttribute>())
      method = typeof (ValueRef<>).MakeGenericType(param.ParameterType.GetElementType()).GetMethod("EvaluateParamsCollection", BindingFlags.Static | BindingFlags.NonPublic);
    else
      method = typeof (ValueRef<>).MakeGenericType(param.ParameterType).GetMethod("EvaluateParameter", BindingFlags.Static | BindingFlags.NonPublic);
    return (Expression) Expression.Call(method, (Expression) indexExpression, (Expression) memberExpression);
  }

  public override string ToString() => this.FullName;

  public string GetHelp()
  {
    string help;
    if (this._loc.TryGetString("command-help-" + this.LocName, out help))
      return help;
    StringBuilder builder = new StringBuilder();
    if (((IEnumerable<CommandMethod>) this.Methods).Any<CommandMethod>((Func<CommandMethod, bool>) (x => x.Invertible)))
      builder.AppendLine(this._loc.GetString("command-help-invertible"));
    builder.Append(this._loc.GetString("command-help-usage"));
    foreach (CommandMethod method in this.Methods)
    {
      builder.Append(Environment.NewLine + "  ");
      ParameterInfo pipeArg = method.PipeArg;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
      if (pipeArg != null)
      {
        string argHint;
        if (!this._loc.TryGetString($"command-arg-sig-{this.LocName}-{pipeArg.Name}", out argHint))
          argHint = ToolshedCommand.GetArgHint(pipeArg.Name, false, false, pipeArg.ParameterType);
        StringBuilder stringBuilder1 = builder;
        StringBuilder stringBuilder2 = stringBuilder1;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder1);
        interpolatedStringHandler.AppendFormatted(argHint);
        interpolatedStringHandler.AppendLiteral(" → ");
        ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
        stringBuilder2.Append(ref local);
      }
      if (method.Invertible)
        builder.Append("[not] ");
      this.AddMethodSignature(builder, method.Arguments);
      if (method.Info.ReturnType != typeof (void))
      {
        StringBuilder stringBuilder3 = builder;
        StringBuilder stringBuilder4 = stringBuilder3;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder3);
        interpolatedStringHandler.AppendLiteral(" → ");
        interpolatedStringHandler.AppendFormatted(method.Info.ReturnType.PrettyName());
        ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
        stringBuilder4.Append(ref local);
      }
    }
    return builder.ToString();
  }

  internal void AddMethodSignature(StringBuilder builder, CommandArgument[] args, Type[]? typeArgs = null)
  {
    builder.Append(this.FullName);
    Type[] parameterParsers = this.Owner.TypeParameterParsers;
    int num = 0;
    foreach (Type parser in parameterParsers)
    {
      if (!(parser == typeof (TypeTypeParser)) && this._toolshed.GetCustomParser(parser).ShowTypeArgSignature)
        ++num;
    }
    if (num > 0)
    {
      builder.Append('<');
      for (int index = 0; index < num; ++index)
      {
        if (index > 0)
          builder.Append(", ");
        if (typeArgs != null)
        {
          builder.Append(typeArgs[index].PrettyName());
        }
        else
        {
          builder.Append('T');
          if (num > 1)
            builder.Append(index + 1);
        }
      }
      builder.Append('>');
    }
    foreach (CommandArgument commandArgument in args)
    {
      builder.Append(' ');
      string str;
      if (this._loc.TryGetString($"command-arg-sig-{this.LocName}-{commandArgument.Name}", out str))
        builder.Append(str);
      else
        builder.Append(ToolshedCommand.GetArgHint(new CommandArgument?(commandArgument), commandArgument.Type));
    }
  }

  public string DescriptionLocKey() => "command-description-" + this.LocName;

  public string Description() => this._loc.GetString(this.DescriptionLocKey());
}
