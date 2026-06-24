// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.BlockTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class BlockTypeParser<T> : TypeParser<Block<T>>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block<T>? result)
  {
    return Block<T>.TryParse(ctx, out result);
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    Block<T>.TryParse(ctx, out Block<T> _);
    return ctx.Completions;
  }
}
