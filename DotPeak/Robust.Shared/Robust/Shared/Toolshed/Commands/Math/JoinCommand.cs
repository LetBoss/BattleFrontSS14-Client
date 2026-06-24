// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.JoinCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class JoinCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public string Join([PipedArgument] string x, string y) => x + y;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Join<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y)
  {
    return x.Concat<T>(y);
  }
}
