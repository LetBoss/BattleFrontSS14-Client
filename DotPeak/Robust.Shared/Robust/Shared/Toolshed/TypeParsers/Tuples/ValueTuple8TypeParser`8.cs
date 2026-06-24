// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Tuples.ValueTuple8TypeParser`8
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple8TypeParser<T1, T2, T3, T4, T5, T6, T7, T8> : 
  BaseTupleTypeParser<(T1, T2, T3, T4, T5, T6, T7, T8)>
  where T8 : unmanaged
{
  public override IEnumerable<Type> Fields
  {
    get
    {
      return (IEnumerable<Type>) new Type[8]
      {
        typeof (T1),
        typeof (T2),
        typeof (T3),
        typeof (T4),
        typeof (T5),
        typeof (T6),
        typeof (T7),
        typeof (T8)
      };
    }
  }

  public override (T1, T2, T3, T4, T5, T6, T7, T8) Create(IReadOnlyList<object> values)
  {
    return ((T1) values[0], (T2) values[1], (T3) values[2], (T4) values[3], (T5) values[4], (T6) values[5], (T7) values[6], (T8) values[7]);
  }
}
