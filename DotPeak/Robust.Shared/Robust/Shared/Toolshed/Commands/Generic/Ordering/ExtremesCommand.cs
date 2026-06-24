// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Ordering.ExtremesCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Ordering;

[ToolshedCommand]
public sealed class ExtremesCommand : ToolshedCommand
{
  [TakesPipedTypeAsGeneric]
  [CommandImplementation(null)]
  public IEnumerable<T> Extremes<T>([PipedArgument] IEnumerable<T> input)
  {
    if (!(input is IList<T> collection))
      collection = (IList<T>) input.ToArray<T>();
    int len = collection.Count;
    for (int i = 0; i < len / 2; ++i)
    {
      yield return collection[i];
      IList<T> objList = collection;
      yield return objList[objList.Count - i];
    }
    if (collection.Count % 2 != 0)
      yield return collection[collection.Count / 2];
  }
}
