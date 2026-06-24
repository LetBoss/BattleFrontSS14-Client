// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.UnitEnumerable`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

internal sealed record UnitEnumerable<T>(T Value) : IEnumerable<T>, IEnumerable
{
  public IEnumerator<T> GetEnumerator()
  {
    return (IEnumerator<T>) new UnitEnumerable<T>.UnitEnumerator(this.Value);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  internal record struct UnitEnumerator(T Value) : IEnumerator<T>, IEnumerator, IDisposable
  {
    private bool _taken = false;

    public T Value { get; set; } = Value;

    public bool MoveNext()
    {
      if (this._taken)
        return false;
      this._taken = true;
      return true;
    }

    public void Reset() => this._taken = false;

    public T Current => this.Value;

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
    }
  }
}
