// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.StopwatchCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class StopwatchCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public object? Stopwatch(IInvocationContext ctx, CommandRun expr)
  {
    Robust.Shared.Timing.Stopwatch stopwatch = new Robust.Shared.Timing.Stopwatch();
    stopwatch.Start();
    object obj = expr.Invoke((object) null, ctx);
    IInvocationContext invocationContext = ctx;
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
    interpolatedStringHandler.AppendLiteral("Ran expression in [color=");
    ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
    Color aqua = Color.Aqua;
    string hex = ((Color) ref aqua).ToHex();
    local.AppendFormatted(hex);
    interpolatedStringHandler.AppendLiteral("]");
    interpolatedStringHandler.AppendFormatted<TimeSpan>(stopwatch.Elapsed, "g");
    interpolatedStringHandler.AppendLiteral("[/color]");
    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
    invocationContext.WriteMarkup(stringAndClear);
    return obj;
  }
}
