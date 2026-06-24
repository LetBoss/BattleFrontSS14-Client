// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Tuples.ValueTuple1TypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple1TypeParser<T1> : BaseTupleTypeParser<ValueTuple<T1>>
{
  public override IEnumerable<Type> Fields
  {
    get => (IEnumerable<Type>) new Type[1]{ typeof (T1) };
  }

  public override ValueTuple<T1> Create(IReadOnlyList<object> values) => ((T1) values[0]);
}
