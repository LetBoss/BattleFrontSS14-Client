// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.IterateCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class IterateCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T>? Iterate<T>(IInvocationContext ctx, [PipedArgument] T value, Block<T, T> block, int times)
  {
    for (int i = 0; i < times; ++i)
    {
      T obj = block.Invoke(value, ctx);
      if ((object) obj == null || ctx.HasErrors)
        break;
      value = obj;
      yield return value;
    }
  }
}
