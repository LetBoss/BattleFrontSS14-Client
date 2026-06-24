// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FixedArray8`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

internal struct FixedArray8<T> : IEquatable<FixedArray8<T>>
{
  public T _00;
  public T _01;
  public T _02;
  public T _03;
  public T _04;
  public T _05;
  public T _06;
  public T _07;

  public Span<T> AsSpan => MemoryMarshal.CreateSpan<T>(ref this._00, 8);

  internal FixedArray8(T x0, T x1, T x2, T x3, T x4, T x5, T x6, T x7)
  {
    this._00 = x0;
    this._01 = x1;
    this._02 = x2;
    this._03 = x3;
    this._04 = x4;
    this._05 = x5;
    this._06 = x6;
    this._07 = x7;
  }

  public bool Equals(FixedArray8<T> other)
  {
    return EqualityComparer<T>.Default.Equals(this._00, other._00) && EqualityComparer<T>.Default.Equals(this._01, other._01) && EqualityComparer<T>.Default.Equals(this._02, other._02) && EqualityComparer<T>.Default.Equals(this._03, other._03) && EqualityComparer<T>.Default.Equals(this._04, other._04) && EqualityComparer<T>.Default.Equals(this._05, other._05) && EqualityComparer<T>.Default.Equals(this._06, other._06) && EqualityComparer<T>.Default.Equals(this._07, other._07);
  }

  public override bool Equals(object? obj) => obj is FixedArray8<T> other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<T, T, T, T, T, T, T, T>(this._00, this._01, this._02, this._03, this._04, this._05, this._06, this._07);
  }
}
