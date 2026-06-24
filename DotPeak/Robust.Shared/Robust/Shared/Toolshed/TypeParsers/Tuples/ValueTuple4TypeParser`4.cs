// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Tuples.ValueTuple4TypeParser`4
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple4TypeParser<T1, T2, T3, T4> : BaseTupleTypeParser<(T1, T2, T3, T4)>
{
  public override IEnumerable<Type> Fields
  {
    get
    {
      return (IEnumerable<Type>) new Type[4]
      {
        typeof (T1),
        typeof (T2),
        typeof (T3),
        typeof (T4)
      };
    }
  }

  public override (T1, T2, T3, T4) Create(IReadOnlyList<object> values)
  {
    return ((T1) values[0], (T2) values[1], (T3) values[2], (T4) values[3]);
  }
}
