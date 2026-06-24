// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.RemQueue`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public struct RemQueue<T>
{
  public System.Collections.Generic.List<T>? List;

  public int Count
  {
    get
    {
      System.Collections.Generic.List<T> list = this.List;
      return list == null ? 0 : __nonvirtual (list.Count);
    }
  }

  public void Add(T t)
  {
    if (this.List == null)
      this.List = new System.Collections.Generic.List<T>();
    this.List.Add(t);
  }

  public RemQueue<
  #nullable disable
  T>.Enumerator GetEnumerator() => new RemQueue<T>.Enumerator(this.List);

  public void Clear() => this.List?.Clear();

  public struct Enumerator : IEnumerator<
  #nullable enable
  T>, IEnumerator, IDisposable
  {
    private readonly bool _filled;
    private System.Collections.Generic.List<T>.Enumerator _enumerator;

    public Enumerator(System.Collections.Generic.List<T>? list)
    {
      if (list == null)
      {
        this._filled = false;
        this._enumerator = new System.Collections.Generic.List<T>.Enumerator();
      }
      else
      {
        this._filled = true;
        this._enumerator = list.GetEnumerator();
      }
    }

    public bool MoveNext() => this._filled && this._enumerator.MoveNext();

    void IEnumerator.Reset()
    {
      if (!this._filled)
        return;
      ((IEnumerator) this._enumerator).Reset();
    }

    public T Current => this._enumerator.Current;

    object? IEnumerator.Current => (object) this.Current;

    void IDisposable.Dispose()
    {
    }
  }
}
