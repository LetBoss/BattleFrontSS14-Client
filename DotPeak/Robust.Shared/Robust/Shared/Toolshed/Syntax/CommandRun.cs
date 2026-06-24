// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.CommandRun
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.TypeParsers.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class CommandRun
{
  public readonly string OriginalExpr;
  public readonly List<(ParsedCommand, Vector2i)> Commands;
  public readonly Type? ReturnType;
  public readonly Type? PipedType;
  public readonly int StartIndex;
  public readonly int EndIndex;

  public string SubExpr
  {
    get
    {
      string originalExpr = this.OriginalExpr;
      int startIndex1 = this.StartIndex;
      int startIndex2 = startIndex1;
      int length = this.EndIndex - startIndex1;
      return originalExpr.Substring(startIndex2, length);
    }
  }

  public CommandRun(
    List<(ParsedCommand, Vector2i)> commands,
    string originalExpr,
    Type? returnType,
    Type? pipedType)
  {
    this.OriginalExpr = originalExpr;
    this.Commands = commands;
    this.ReturnType = returnType;
    this.PipedType = pipedType;
    this.StartIndex = commands[0].Item2.X;
    List<(ParsedCommand, Vector2i)> tupleList = commands;
    this.EndIndex = tupleList[tupleList.Count - 1].Item2.Y;
  }

  public static bool TryParse(
    ParserContext ctx,
    Type? pipedType,
    Type? targetOutput,
    [NotNullWhen(true)] out CommandRun? expr)
  {
    expr = (CommandRun) null;
    List<(ParsedCommand, Vector2i)> commands = new List<(ParsedCommand, Vector2i)>();
    ctx.ConsumeWhitespace();
    if (pipedType == typeof (void))
      throw new ArgumentException("Piped type cannot be void");
    int index = ctx.Index;
    if (ctx.PeekBlockTerminator())
    {
      ctx.Error = (IConError) new EmptyCommandRun();
      ctx.Error.Contextualize(ctx.Input, new Vector2i(index, ctx.Index + 1));
      return false;
    }
    ParsedCommand result;
    while (ParsedCommand.TryParse(ctx, pipedType, out result))
    {
      pipedType = result.ReturnType;
      commands.Add((result, Vector2i.op_Implicit((index, ctx.Index))));
      ctx.ConsumeWhitespace();
      bool commandExpected;
      ctx.EatCommandTerminators(ref pipedType, out commandExpected);
      if (ctx.PeekBlockTerminator())
      {
        if (commandExpected)
        {
          if (!ctx.GenerateCompletions)
          {
            ctx.Error = (IConError) new UnexpectedCloseBrace();
            ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
          }
          return false;
        }
      }
      else if (ctx.OutOfInput)
      {
        if (commandExpected)
        {
          if (ctx.GenerateCompletions)
            ParsedCommand.TryParse(ctx, pipedType, out ParsedCommand _);
          else
            ctx.Error = (IConError) new OutOfInputError();
          return false;
        }
      }
      else
      {
        index = ctx.Index;
        if (!(pipedType != typeof (void)))
        {
          if (ctx.GenerateCompletions)
            return false;
          ctx.Error = (IConError) new EndOfCommandError();
          ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
          return false;
        }
        continue;
      }
      if (ctx.Error != null || commands.Count == 0)
      {
        expr = (CommandRun) null;
        return false;
      }
      Type type1;
      if (!(pipedType != (Type) null))
      {
        type1 = typeof (void);
      }
      else
      {
        List<(ParsedCommand, Vector2i)> valueTupleList = commands;
        type1 = valueTupleList[valueTupleList.Count - 1].Item1.ReturnType;
      }
      Type type2 = type1;
      if (targetOutput != (Type) null && !type2.IsAssignableTo(targetOutput))
      {
        ctx.Error = (IConError) new WrongCommandReturn(targetOutput, type2);
        expr = (CommandRun) null;
        return false;
      }
      expr = new CommandRun(commands, ctx.Input, type2, pipedType);
      return true;
    }
    if (ctx.Error is NotValidCommandError error)
      error.TargetType = targetOutput;
    return false;
  }

  public object? Invoke(object? input, IInvocationContext ctx, bool reportErrors = true)
  {
    if (ctx.HasErrors)
      throw new Exception("Improperly handled Toolshed errors");
    object pipedIn = input;
    foreach ((ParsedCommand parsedCommand, Vector2i issueSpan) in this.Commands)
    {
      pipedIn = parsedCommand.Invoke(pipedIn, ctx);
      if (ctx.HasErrors)
      {
        if (!reportErrors)
          return (object) null;
        foreach (IConError error in ctx.GetErrors())
        {
          error.Contextualize(this.OriginalExpr, issueSpan);
          ctx.WriteLine(error.Describe());
        }
        return (object) null;
      }
    }
    return pipedIn;
  }

  public override string ToString() => this.SubExpr;
}
