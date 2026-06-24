// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Collections.RingBufferList`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Collections;

internal sealed class RingBufferList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  private T[] _items;
  private int _read;
  private int _write;

  public RingBufferList(int capacity) => this._items = new T[capacity];

  public RingBufferList() => this._items = Array.Empty<T>();

  public int Capacity => this._items.Length;

  private bool IsFull => this._items.Length == 0 || this.NextIndex(this._write) == this._read;

  public void Add(T item)
  {
    if (this.IsFull)
      this.Expand();
    this._items[this._write] = item;
    this._write = this.NextIndex(this._write);
  }

  public void Clear()
  {
    this._read = 0;
    this._write = 0;
    if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      return;
    Array.Clear((Array) this._items);
  }

  public bool Contains(T item) => this.IndexOf(item) >= 0;

  public void CopyTo(T[] array, int arrayIndex)
  {
    ArgumentNullException.ThrowIfNull((object) array, nameof (array));
    ArgumentOutOfRangeException.ThrowIfNegative<int>(arrayIndex, nameof (arrayIndex));
    this.CopyTo(array.AsSpan<T>(arrayIndex));
  }

  private void CopyTo(Span<T> dest)
  {
    // ISSUE: unable to decompile the method.
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index < 0)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public int Count
  {
    get
    {
      int num = this._write - this._read;
      return num >= 0 ? num : num + this._items.Length;
    }
  }

  public bool IsReadOnly => false;

  public int IndexOf(T item)
  {
    // ISSUE: unable to decompile the method.
  }

  public void Insert(int index, T item) => throw new NotSupportedException();

  public void RemoveAt(int index)
  {
    int count = this.Count;
    ArgumentOutOfRangeException.ThrowIfNegative<int>(index, nameof (index));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<int>(index, count, nameof (index));
    if (index == 0)
    {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        this._items[this._read] = default (T);
      this._read = this.NextIndex(this._read);
    }
    else if (index == count - 1)
    {
      this._write = this.WrapInv(this._write - 1);
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        return;
      this._items[this._write] = default (T);
    }
    else
    {
      int start1 = this.RealIndex(index);
      ref T local1 = ref this._items[start1];
      if (start1 < this._read)
      {
        Span<T> span = this._items.AsSpan<T>();
        ref Span<T> local2 = ref span;
        int num = start1;
        int start2 = num;
        int length = this._write - num;
        RingBufferList<T>.ShiftDown(local2.Slice(start2, length), default (T));
      }
      else if (this._write < this._read)
      {
        T substitution = RingBufferList<T>.ShiftDown(this._items.AsSpan<T>(0, this._write), default (T));
        RingBufferList<T>.ShiftDown(this._items.AsSpan<T>(start1), substitution);
      }
      else
      {
        Span<T> span = this._items.AsSpan<T>();
        ref Span<T> local3 = ref span;
        int num = start1;
        int start3 = num;
        int length = this._write - num;
        RingBufferList<T>.ShiftDown(local3.Slice(start3, length), default (T));
      }
      this._write = this.WrapInv(this._write - 1);
    }
  }

  private static T ShiftDown(Span<T> span, T substitution)
  {
    if (span.Length == 0)
      return substitution;
    T obj = span[0];
    ref Span<T> local1 = ref span;
    Span<T> span1 = local1.Slice(1, local1.Length - 1);
    ref Span<T> local2 = ref span1;
    ref Span<T> local3 = ref span;
    Span<T> destination = local3.Slice(0, local3.Length - 1);
    local2.CopyTo(destination);
    ref Span<T> local4 = ref span;
    local4[local4.Length - 1] = substitution;
    return obj;
  }

  public T this[int index]
  {
    get => this.GetSlot(index);
    set => this.GetSlot(index) = value;
  }

  private ref T GetSlot(int index)
  {
    ArgumentOutOfRangeException.ThrowIfNegative<int>(index, nameof (index));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<int>(index, this.Count, nameof (index));
    return ref this._items[this.RealIndex(index)];
  }

  private int RealIndex(int index) => this.Wrap(index + this._read);

  private int NextIndex(int index) => this.Wrap(index + 1);

  private int Wrap(int index)
  {
    if (index >= this._items.Length)
      index -= this._items.Length;
    return index;
  }

  private int WrapInv(int index)
  {
    if (index < 0)
      index = this._items.Length - 1;
    return index;
  }

  private void Expand()
  {
    int length = this._items.Length;
    Array.Resize<T>(ref this._items, Math.Max(4, length * 2));
    if (this._write >= this._read)
      return;
    Span<T> span = this._items.AsSpan<T>(0, this._write);
    Span<T> destination = this._items.AsSpan<T>(length);
    span.CopyTo(destination);
    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      span.Clear();
    this._write += length;
  }

  public RingBufferList<
  #nullable disable
  T>.Enumerator GetEnumerator() => new RingBufferList<T>.Enumerator(this);

  #nullable enable
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>) this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly RingBufferList<T> _ringBufferList;
    private int _readPos;

    internal Enumerator(RingBufferList<T> ringBufferList)
    {
      this._ringBufferList = ringBufferList;
      this._readPos = this._ringBufferList._read - 1;
    }

    public bool MoveNext()
    {
      this._readPos = this._ringBufferList.NextIndex(this._readPos);
      return this._readPos != this._ringBufferList._write;
    }

    public void Reset() => this = new RingBufferList<T>.Enumerator(this._ringBufferList);

    public ref T Current => ref this._ringBufferList._items[this._readPos];

    T IEnumerator<T>.Current => this.Current;

    object? IEnumerator.Current => (object) this.Current;

    void IDisposable.Dispose()
    {
    }
  }
}
