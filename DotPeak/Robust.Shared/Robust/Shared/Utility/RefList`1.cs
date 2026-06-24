// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.RefList`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class RefList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  private T[] _array;
  private int _size;

  public RefList()
    : this(1)
  {
  }

  public RefList(int initialCapacity)
  {
    this._array = new T[initialCapacity];
    this._size = 0;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public RefList<
  #nullable disable
  T>.Enumerator GetEnumerator() => new RefList<T>.Enumerator(this);

  #nullable enable
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>) this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public ref T AllocAdd()
  {
    this._ensureCapacity(this._size + 1);
    return ref this._array[this._size++];
  }

  public void Add(T item)
  {
    this._ensureCapacity(this._size + 1);
    this._array[this._size++] = item;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Clear()
  {
    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      Array.Clear((Array) this._array, 0, this._size);
    this._size = 0;
  }

  public bool Contains(T item) => this.IndexOf(item) != -1;

  public void CopyTo(T[] array, int arrayIndex)
  {
    Array.Copy((Array) this._array, 0, (Array) array, arrayIndex, this._size);
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index == -1)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public int Count => this._size;

  public bool IsReadOnly => false;

  public int Capacity => this._array.Length;

  public int IndexOf(T item) => Array.IndexOf<T>(this._array, item, 0, this._size);

  public void Insert(int index, T item)
  {
    this._ensureCapacity(this._size + 1);
    if (index < this._size)
      Array.Copy((Array) this._array, index, (Array) this._array, index + 1, this._size - index);
    this._array[index] = item;
    ++this._size;
  }

  public void TrimCapacity(int capacity)
  {
    if (this.Count > capacity)
      throw new ArgumentException("Cannot trim past list contents");
    T[] array = this._array;
    this._array = new T[capacity];
    int size = this._size;
    array.AsSpan<T>(0, size).CopyTo((Span<T>) this._array);
  }

  public void RemoveAt(int index)
  {
    if (index >= this._size)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Index must fit into list.");
    --this._size;
    if (index < this._size)
      Array.Copy((Array) this._array, index + 1, (Array) this._array, index, this._size - index);
    if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      return;
    this._array[this._size] = default (T);
  }

  public ref T this[int index]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref this._array[index];
  }

  T IList<T>.this[int index]
  {
    get => this._array[index];
    set => this._array[index] = value;
  }

  public void Sort(IComparer<T> comparer) => Array.Sort<T>(this._array, 0, this._size, comparer);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Span<T> GetSpan() => new Span<T>(this._array, 0, this._size);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void _ensureCapacity(int newCapacity)
  {
    if (newCapacity < this._array.Length)
      return;
    T[] array = this._array;
    this._array = new T[array.Length * 2];
    Array.Copy((Array) array, 0, (Array) this._array, 0, this._size);
  }

  [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
  public struct Enumerator(RefList<T> owner) : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly RefList<T> _owner = owner;
    private int _position = -1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => ++this._position < this._owner._size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => this._position = 0;

    public ref T Current
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get
      {
        return ref this._owner._array[this._position];
      }
    }

    T IEnumerator<T>.Current => this.Current;

    object? IEnumerator.Current => (object) this.Current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
    }
  }
}
