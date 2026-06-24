// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.CustomValueRefTypeParser`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class CustomValueRefTypeParser<T, TParser> : CustomTypeParser<ValueRef<T>>
  where T : notnull
  where TParser : CustomTypeParser<T>, new()
{
  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    TParser customParser = this.Toolshed.GetCustomParser<TParser, T>();
    return ValueRefTypeParser<T>.TryAutocomplete(this.Toolshed, ctx, arg, (ITypeParser) customParser);
  }

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T>? result)
  {
    TParser customParser = this.Toolshed.GetCustomParser<TParser, T>();
    return ValueRefTypeParser<T>.TryParse(this.Toolshed, ctx, (ITypeParser) customParser, out result);
  }
}
