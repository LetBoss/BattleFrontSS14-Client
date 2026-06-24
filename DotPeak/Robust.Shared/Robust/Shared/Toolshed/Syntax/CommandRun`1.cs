// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.CommandRun`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class CommandRun<TRes>
{
  internal readonly CommandRun InnerCommandRun;

  public static bool TryParse(ParserContext ctx, Type? pipedType, [NotNullWhen(true)] out CommandRun<TRes>? expr)
  {
    CommandRun expr1;
    if (!CommandRun.TryParse(ctx, pipedType, typeof (TRes), out expr1))
    {
      expr = (CommandRun<TRes>) null;
      return false;
    }
    expr = new CommandRun<TRes>(expr1);
    return true;
  }

  public TRes? Invoke(object? input, IInvocationContext ctx)
  {
    object obj = this.InnerCommandRun.Invoke(input, ctx);
    return obj == null ? default (TRes) : (TRes) obj;
  }

  internal CommandRun(CommandRun commandRun) => this.InnerCommandRun = commandRun;

  public override string ToString() => this.InnerCommandRun.ToString();
}
