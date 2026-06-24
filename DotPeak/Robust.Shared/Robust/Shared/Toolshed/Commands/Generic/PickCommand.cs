// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.PickCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class PickCommand : ToolshedCommand
{
  [Dependency]
  private readonly IRobustRandom _random;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Pick<T>([PipedArgument] IEnumerable<T> input)
  {
    return RandomExtensions.Pick<T>(this._random, (IReadOnlyList<T>) input.ToArray<T>());
  }
}
