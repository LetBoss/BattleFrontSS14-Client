// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Ordering.SortCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Ordering;

[ToolshedCommand]
public sealed class SortCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Sort<T>([PipedArgument] IEnumerable<T> input) where T : IComparable<T>
  {
    return (IEnumerable<T>) input.Order<T>();
  }
}
