// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Collections.ValueList`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Collections;

public struct ValueList<T> : IEnumerable<T>, IEnumerable
{
  private const int DefaultCapacity = 4;
  private T[]? _items;

  public ValueList(int capacity)
  {
    this._items = capacity == 0 ? (T[]) null : new T[capacity];
    this.Count = 0;
  }

  public ValueList(List<T> list)
    : this(list, 0, list.Count)
  {
  }

  public ValueList(List<T> list, int start, int count)
  {
    this._items = new T[count];
    System.Span<T> span = CollectionsMarshal.AsSpan<T>(list);
    ref System.Span<T> local = ref span;
    int num = start;
    int start1 = num;
    int length = start + count - num;
    local.Slice(start1, length).CopyTo((System.Span<T>) this._items);
    this.Count = count;
  }

  public ValueList(IReadOnlyCollection<T> list)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CCount\u003Ek__BackingField = 0;
    this._items = new T[list.Count];
    foreach (T obj in (IEnumerable<T>) list)
      this.AddNoResize(obj, this.Count);
  }

  public ValueList(IEnumerable<T> collection)
  {
    this._items = collection.ToArray<T>();
    this.Count = this._items.Length;
  }

  public static ValueList<T> OwningArray(T[]? array)
  {
    ValueList<T> valueList = new ValueList<T>()
    {
      _items = array
    };
    valueList.Count = valueList.Capacity;
    return valueList;
  }

  public static ValueList<T> OwningArray(T[]? array, int count)
  {
    ValueList<T> valueList = new ValueList<T>();
    valueList._items = array;
    if (count < 0)
      throw new ArgumentException("Count cannot be negative.");
    if (count >= valueList.Capacity)
      throw new ArgumentException("Count cannot be greater than the size of the array.");
    valueList.Count = count;
    return valueList;
  }

  public int Count { get; private set; }

  public readonly ref T this[int index]
  {
    get
    {
      if ((uint) index >= (uint) this.Count)
        throw new IndexOutOfRangeException();
      return ref this._items[index];
    }
  }

  public int Capacity
  {
    readonly get
    {
      T[] items = this._items;
      return items == null ? 0 : items.Length;
    }
    set
    {
      if (value < this.Count)
        throw new ArgumentException("Cannot set capacity lower than contained count");
      if (value == this.Capacity)
        return;
      if (value > 0)
      {
        T[] destinationArray = new T[value];
        if (this.Count > 0)
          Array.Copy((Array) this._items, (Array) destinationArray, this.Count);
        this._items = destinationArray;
      }
      else
        this._items = (T[]) null;
    }
  }

  public readonly System.Span<T> Span => new System.Span<T>(this._items, 0, this.Count);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Add(T item)
  {
    int count = this.Count;
    if ((uint) count < (uint) this.Capacity)
      this.AddNoResize(item, count);
    else
      this.AddWithResize(item);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void AddNoResize(T item, int size)
  {
    T[] items = this._items;
    this.Count = size + 1;
    int index = size;
    T obj = item;
    items[index] = obj;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ref T AddRef()
  {
    T[] items = this._items;
    int count = this.Count;
    if ((uint) count >= (uint) this.Capacity)
      return ref this.AddRefWithResize();
    this.Count = count + 1;
    return ref items[count];
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void AddWithResize(T item)
  {
    int count = this.Count;
    this.Grow(count + 1);
    this.Count = count + 1;
    this._items[count] = item;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private ref T AddRefWithResize()
  {
    int count = this.Count;
    this.Grow(count + 1);
    this.Count = count + 1;
    return ref this._items[count];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Clear()
  {
    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
    {
      int count = this.Count;
      this.Count = 0;
      if (count <= 0)
        return;
      Array.Clear((Array) this._items, 0, count);
    }
    else
      this.Count = 0;
  }

  public readonly bool Contains(T item) => this.IndexOf(item) >= 0;

  public int EnsureCapacity(int capacity)
  {
    if (capacity < 0)
      throw new ArgumentException("Capacity cannot be negative");
    if (this.Capacity < capacity)
      this.Grow(capacity);
    return capacity == 0 ? capacity : this._items.Length;
  }

  private void Grow(int capacity)
  {
    int num = this.Capacity == 0 ? 4 : 2 * this._items.Length;
    if ((long) (uint) num > (long) Array.MaxLength)
      num = Array.MaxLength;
    if (num < capacity)
      num = capacity;
    this.Capacity = num;
  }

  public readonly ValueList<
  #nullable disable
  T>.Enumerator GetEnumerator() => new ValueList<T>.Enumerator(this);

  #nullable enable
  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return (IEnumerator<T>) new ValueList<T>.Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new ValueList<T>.Enumerator(this);

  public readonly int IndexOf(T item)
  {
    return this._items != null ? Array.IndexOf<T>(this._items, item, 0, this.Count) : -1;
  }

  public readonly int IndexOf(T item, int index)
  {
    if (index > this.Count)
      throw new ArgumentOutOfRangeException();
    return this._items != null ? Array.IndexOf<T>(this._items, item, index, this.Count - index) : -1;
  }

  public readonly int IndexOf(T item, int index, int count)
  {
    if (index > this.Count)
      throw new ArgumentException("Start index out of bounds");
    if (count < 0 || index > this.Count - count)
      throw new ArgumentException("Count out of range");
    return this._items != null ? Array.IndexOf<T>(this._items, item, index, count) : -1;
  }

  public void Insert(int index, T item)
  {
    if ((uint) index > (uint) this.Count)
      throw new ArgumentOutOfRangeException();
    if (this.Count == this._items.Length)
      this.Grow(this.Count + 1);
    if (index < this.Count)
      Array.Copy((Array) this._items, index, (Array) this._items, index + 1, this.Count - index);
    this._items[index] = item;
    ++this.Count;
  }

  public readonly int LastIndexOf(T item)
  {
    return this.Count == 0 ? -1 : this.LastIndexOf(item, this.Count - 1, this.Count);
  }

  public readonly int LastIndexOf(T item, int index)
  {
    if (index >= this.Count)
      throw new ArgumentOutOfRangeException(nameof (index), "Index out of range");
    return this.LastIndexOf(item, index, index + 1);
  }

  public readonly int LastIndexOf(T item, int index, int count)
  {
    if (this.Count == 0)
      return -1;
    if (index < 0)
      throw new ArgumentException("Index cannot be negative");
    if (count < 0)
      throw new ArgumentException("Count cannot be negative");
    if (index >= this.Count)
      throw new ArgumentException("Range outside of collection bounds");
    if (count > index + 1)
      throw new ArgumentException("Range outside of collection bounds");
    return Array.LastIndexOf<T>(this._items, item, index, count);
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index < 0)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public void RemoveAt(int index)
  {
    if ((uint) index >= (uint) this.Count)
      throw new ArgumentOutOfRangeException(nameof (index));
    --this.Count;
    if (index < this.Count)
      Array.Copy((Array) this._items, index + 1, (Array) this._items, index, this.Count - index);
    if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      return;
    this._items[this.Count] = default (T);
  }

  public void Sort() => this.Span.Sort<T>();

  public void Sort(IComparer<T>? comparer) => this.Span.Sort<T, IComparer<T>>(comparer);

  public void Sort(Comparison<T> comparison) => this.Span.Sort<T>(comparison);

  public readonly T[] ToArray() => this.Span.ToArray();

  public void TrimExcess()
  {
    if (this.Count >= (int) ((double) this.Capacity * 0.9))
      return;
    this.Capacity = this.Count;
  }

  public T RemoveSwap(int index)
  {
    T obj1 = this[index];
    T obj2 = this[this.Count - 1];
    this[index] = obj2;
    this.RemoveAt(this.Count - 1);
    return obj1;
  }

  public void AddRange(ValueList<T> list) => this.AddRange(list.Span);

  public void AddRange(List<T> list) => this.AddRange(CollectionsMarshal.AsSpan<T>(list));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AddRange(System.Span<T> span) => this.AddRange((ReadOnlySpan<T>) span);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AddRange(ReadOnlySpan<T> span)
  {
    int length = span.Length;
    this.EnsureCapacity(this.Count + length);
    System.Span<T> destination = new System.Span<T>(this._items, this.Count, length);
    span.CopyTo(destination);
    this.Count += length;
  }

  public void EnsureLength(int newCount)
  {
    if (this.Count > newCount)
      return;
    this.EnsureCapacity(newCount);
    new System.Span<T>(this._items, this.Count, newCount - this.Count).Clear();
    this.Count = newCount;
  }

  public void AddRange(IEnumerable<T> select)
  {
    foreach (T obj in select)
      this.Add(obj);
  }

  public void Push(T item) => this.Add(item);

  public T Pop()
  {
    T obj;
    if (!this.TryPop(out obj))
      throw new InvalidOperationException("List is empty");
    return obj;
  }

  public T Peek()
  {
    T obj;
    if (!this.TryPeek(out obj))
      throw new InvalidOperationException("List is empty");
    return obj;
  }

  public bool TryPop([MaybeNullWhen(false)] out T value)
  {
    if (this.Count == 0)
    {
      value = default (T);
      return false;
    }
    value = this._items[--this.Count];
    return true;
  }

  public bool TryPeek([MaybeNullWhen(false)] out T value)
  {
    if (this.Count == 0)
    {
      value = default (T);
      return false;
    }
    value = this._items[this.Count];
    return true;
  }

  public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly ValueList<T> _list;
    private int _index;

    internal Enumerator(ValueList<T> list)
    {
      this._index = -1;
      this._list = list;
    }

    public void Dispose()
    {
    }

    public bool MoveNext() => ++this._index < this._list.Count;

    public T Current => this.RefCurrent;

    public ref T RefCurrent => ref this._list._items[this._index];

    object? IEnumerator.Current => (object) this.Current;

    void IEnumerator.Reset() => this._index = -1;
  }
}
