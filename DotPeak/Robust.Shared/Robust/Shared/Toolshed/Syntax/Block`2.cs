// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.Block`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

[Virtual]
public class Block<TIn, TOut>(CommandRun expr) : Block(expr)
{
  public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block<TIn, TOut>? block)
  {
    block = (Block<TIn, TOut>) null;
    CommandRun run;
    if (!Block.TryParseBlock(ctx, typeof (TIn), typeof (TOut), out run))
      return false;
    block = new Block<TIn, TOut>(run);
    return true;
  }

  public TOut? Invoke(TIn? input, IInvocationContext ctx)
  {
    object obj = this.Run.Invoke((object) input, ctx);
    return obj == null ? default (TOut) : (TOut) obj;
  }
}
