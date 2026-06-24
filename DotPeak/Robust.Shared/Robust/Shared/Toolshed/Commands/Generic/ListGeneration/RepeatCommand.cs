// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.ListGeneration.RepeatCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.ListGeneration;

[ToolshedCommand(Name = "rep")]
public sealed class RepeatCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Repeat<T>([PipedArgument] T value, int amount)
  {
    return Enumerable.Repeat<T>(value, amount);
  }
}
