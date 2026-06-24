// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.WhereCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class WhereCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Where<T>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<T> input,
    Block<T, bool> check)
  {
    foreach (T input1 in input)
    {
      bool flag = check.Invoke(input1, ctx);
      if (ctx.HasErrors)
        break;
      if (flag)
        yield return input1;
    }
  }
}
