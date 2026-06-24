// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ValueRefTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ValueRefTypeParser<T> : TypeParser<ValueRef<T>>
{
  internal static bool TryParse(
    ToolshedManager shed,
    ParserContext ctx,
    ITypeParser? parser,
    [NotNullWhen(true)] out ValueRef<T>? result)
  {
    result = (ValueRef<T>) null;
    ctx.ConsumeWhitespace();
    Rune? nullable1 = ctx.PeekRune();
    Rune? nullable2 = nullable1;
    Rune rune = new Rune('$');
    if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == rune ? 1 : 0) : 0) != 0)
    {
      VarRef<T> parsed;
      if (!shed.TryParse<VarRef<T>>(ctx, out parsed))
        return false;
      result = (ValueRef<T>) parsed;
      return true;
    }
    Rune? nullable3 = nullable1;
    rune = new Rune('{');
    if ((nullable3.HasValue ? (nullable3.GetValueOrDefault() == rune ? 1 : 0) : 0) != 0)
    {
      Block<T> parsed;
      if (!shed.TryParse<Block<T>>(ctx, out parsed))
        return false;
      result = (ValueRef<T>) new BlockRef<T>(parsed);
      return true;
    }
    if (parser == null)
      parser = shed.GetParserForType(typeof (T));
    if (parser == null)
    {
      if (!ctx.GenerateCompletions)
        ctx.Error = (IConError) new MustBeVarOrBlock(typeof (T));
      return false;
    }
    object result1;
    if (!parser.TryParse(ctx, out result1) || !(result1 is T obj))
      return false;
    result = (ValueRef<T>) new ParsedValueRef<T>(obj);
    return true;
  }

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T>? result)
  {
    return ValueRefTypeParser<T>.TryParse(this.Toolshed, ctx, (ITypeParser) null, out result);
  }

  public static CompletionResult? TryAutocomplete(
    ToolshedManager shed,
    ParserContext ctx,
    CommandArgument? arg,
    ITypeParser? parser)
  {
    ctx.ConsumeWhitespace();
    Rune? nullable1 = ctx.PeekRune();
    Rune? nullable2 = nullable1;
    Rune rune = new Rune('$');
    if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == rune ? 1 : 0) : 0) != 0)
      return shed.TryAutocomplete(ctx, typeof (VarRef<T>), arg);
    Rune? nullable3 = nullable1;
    rune = new Rune('{');
    if ((nullable3.HasValue ? (nullable3.GetValueOrDefault() == rune ? 1 : 0) : 0) != 0)
    {
      Block<T>.TryParse(ctx, out Block<T> _);
      return ctx.Completions;
    }
    if (parser == null)
      parser = shed.GetParserForType(typeof (T));
    if (parser == null)
      return CompletionResult.FromHint($"<variable or block of type {typeof (T).PrettyName()}>");
    CompletionResult completionResult = parser.TryAutocomplete(ctx, arg);
    return (object) completionResult != null ? completionResult : CompletionResult.FromHint($"<variable, block, or value of type {typeof (T).PrettyName()}>");
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    return ValueRefTypeParser<T>.TryAutocomplete(this.Toolshed, ctx, arg, (ITypeParser) null);
  }
}
