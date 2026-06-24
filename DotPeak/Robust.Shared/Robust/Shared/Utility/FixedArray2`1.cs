// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FixedArray2`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

internal struct FixedArray2<T> : IEquatable<FixedArray2<T>>
{
  public T _00;
  public T _01;

  public Span<T> AsSpan => MemoryMarshal.CreateSpan<T>(ref this._00, 2);

  internal FixedArray2(T x0, T x1)
  {
    this._00 = x0;
    this._01 = x1;
  }

  public bool Equals(FixedArray2<T> other)
  {
    return EqualityComparer<T>.Default.Equals(this._00, other._00) && EqualityComparer<T>.Default.Equals(this._01, other._01);
  }

  public override bool Equals(object? obj) => obj is FixedArray2<T> other && this.Equals(other);

  public override int GetHashCode() => HashCode.Combine<T, T>(this._00, this._01);
}
