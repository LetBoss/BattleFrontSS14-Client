// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ValueRefTypeParser`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ValueRefTypeParser<T, TAuto> : TypeParser<ValueRef<T, TAuto>>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T, TAuto>? result)
  {
    ValueRef<T> parsed;
    int num = this.Toolshed.TryParse<ValueRef<T>>(ctx, out parsed) ? 1 : 0;
    result = (ValueRef<T, TAuto>) null;
    if (num == 0)
      return num != 0;
    result = new ValueRef<T, TAuto>(parsed);
    return num != 0;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return this.Toolshed.TryAutocomplete(parserContext, typeof (ValueRef<T, T>), arg);
  }
}
