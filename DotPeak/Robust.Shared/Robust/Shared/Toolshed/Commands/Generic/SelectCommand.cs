// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.SelectCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class SelectCommand : ToolshedCommand
{
  [Dependency]
  private readonly IRobustRandom _random;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<TR> Select<TR>([PipedArgument] IEnumerable<TR> enumerable, Quantity quantity, [CommandInverted] bool inverted)
  {
    TR[] array = enumerable.ToArray<TR>();
    this._random.Shuffle<TR>((Span<TR>) array);
    float? amount = quantity.Amount;
    if (amount.HasValue)
    {
      int count = (int) Math.Ceiling((double) amount.GetValueOrDefault());
      if (inverted)
        count = Math.Max(0, array.Length - count);
      return ((IEnumerable<TR>) array).Take<TR>(count);
    }
    int count1 = inverted ? (int) Math.Floor((double) array.Length * Math.Clamp(1.0 - (double) quantity.Percentage.Value, 0.0, 1.0)) : (int) Math.Floor((double) array.Length * Math.Clamp((double) quantity.Percentage.Value, 0.0, 1.0));
    return ((IEnumerable<TR>) array).Take<TR>(count1);
  }
}
