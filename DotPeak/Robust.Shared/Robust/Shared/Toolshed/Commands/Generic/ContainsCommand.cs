// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.ContainsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
internal sealed class ContainsCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public bool Contains<T>([PipedArgument] IEnumerable<T> input, T value, [CommandInverted] bool inverted)
  {
    return inverted ^ input.Contains<T>(value);
  }
}
