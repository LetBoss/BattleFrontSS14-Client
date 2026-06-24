// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.CommandRun`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class CommandRun<TIn, TOut>
{
  internal readonly CommandRun InnerCommandRun;

  public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun<TIn, TOut>? expr)
  {
    CommandRun expr1;
    if (!CommandRun.TryParse(ctx, typeof (TIn), typeof (TOut), out expr1))
    {
      expr = (CommandRun<TIn, TOut>) null;
      return false;
    }
    expr = new CommandRun<TIn, TOut>(expr1);
    return true;
  }

  public TOut? Invoke(object? input, IInvocationContext ctx)
  {
    object obj = this.InnerCommandRun.Invoke(input, ctx);
    return obj == null ? default (TOut) : (TOut) obj;
  }

  internal CommandRun(CommandRun commandRun) => this.InnerCommandRun = commandRun;

  public override string ToString() => this.InnerCommandRun.ToString();
}
