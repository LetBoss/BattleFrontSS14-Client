// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Tuples.ValueTuple3TypeParser`3
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple3TypeParser<T1, T2, T3> : BaseTupleTypeParser<(T1, T2, T3)>
{
  public override IEnumerable<Type> Fields
  {
    get
    {
      return (IEnumerable<Type>) new Type[3]
      {
        typeof (T1),
        typeof (T2),
        typeof (T3)
      };
    }
  }

  public override (T1, T2, T3) Create(IReadOnlyList<object> values)
  {
    return ((T1) values[0], (T2) values[1], (T3) values[2]);
  }
}
