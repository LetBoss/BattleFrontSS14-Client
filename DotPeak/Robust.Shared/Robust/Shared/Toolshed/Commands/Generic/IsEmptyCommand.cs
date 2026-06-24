// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.IsEmptyCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class IsEmptyCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public bool IsEmpty<T>([PipedArgument] T? input, [CommandInverted] bool inverted)
  {
    if ((object) input == null)
      return !inverted;
    return input is IEnumerable source ? !source.Cast<object>().Any<object>() ^ inverted : inverted;
  }
}
