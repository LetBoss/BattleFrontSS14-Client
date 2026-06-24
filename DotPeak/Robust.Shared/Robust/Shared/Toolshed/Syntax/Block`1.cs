// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.Block`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

[Virtual]
public class Block<T>(CommandRun expr) : Block(expr)
{
  public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block<T>? block)
  {
    block = (Block<T>) null;
    CommandRun run;
    if (!Block.TryParseBlock(ctx, (Type) null, typeof (T), out run))
      return false;
    block = new Block<T>(run);
    return true;
  }

  public T? Invoke(IInvocationContext ctx)
  {
    object obj = this.Run.Invoke((object) null, ctx);
    return obj == null ? default (T) : (T) obj;
  }
}
