// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.PriorityQueue`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class PriorityQueue<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
  private readonly IComparer<T> _comparer;
  private T[] _heap;
  private const int DEFAULT_CAPACITY = 10;
  private const int SHRINK_RATIO = 4;
  private const int RESIZE_FACTOR = 2;
  private int _shrinkBound;
  private static readonly InvalidOperationException EmptyCollectionException = new InvalidOperationException("Collection is empty.");

  public PriorityQueue(IComparer<T>? comparer = null)
    : this(10, comparer)
  {
  }

  public PriorityQueue(int capacity, IComparer<T>? comparer = null)
  {
    if (capacity <= 0)
      throw new ArgumentOutOfRangeException(nameof (capacity), "Expected capacity greater than zero.");
    if (comparer == null && !typeof (IComparable).IsAssignableFrom(typeof (T)) && !typeof (IComparable<T>).IsAssignableFrom(typeof (T)))
      throw new ArgumentException("Expected a comparer for types, which do not implement IComparable.", nameof (comparer));
    this._comparer = comparer ?? (IComparer<T>) Comparer<T>.Default;
    this._shrinkBound = capacity / 4;
    this._heap = new T[capacity];
  }

  public int Capacity => this._heap.Length;

  public IEnumerator<T> GetEnumerator()
  {
    T[] array = new T[this.Count];
    this.CopyTo(array, 0);
    return ((IEnumerable<T>) array).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(T item)
  {
    if (this.Count == this.Capacity)
      this.GrowCapacity();
    this._heap[this.Count++] = item;
    this._heap.Sift<T>(this.Count, this._comparer, -1);
  }

  public T Take()
  {
    if (this.Count == 0)
      throw PriorityQueue<T>.EmptyCollectionException;
    T obj = this._heap[0];
    --this.Count;
    this._heap.Swap<T>(0, this.Count);
    this._heap[this.Count] = default (T);
    this._heap.Sink<T>(1, this.Count, this._comparer, -1);
    if (this.Count > this._shrinkBound || this.Count <= 10)
      return obj;
    this.ShrinkCapacity();
    return obj;
  }

  public T Peek()
  {
    if (this.Count == 0)
      throw PriorityQueue<T>.EmptyCollectionException;
    return this._heap[0];
  }

  public void Clear()
  {
    this._heap = new T[10];
    this.Count = 0;
  }

  public bool Contains(T item) => this.GetItemIndex(item) >= 0;

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (arrayIndex < 0)
      throw new ArgumentOutOfRangeException(nameof (arrayIndex));
    if (array.Length - arrayIndex < this.Count)
      throw new ArgumentException("Insufficient space in destination array.");
    Array.Copy((Array) this._heap, 0, (Array) array, arrayIndex, this.Count);
    array.HeapSort<T>(arrayIndex, this.Count, this._comparer);
  }

  public bool Remove(T item)
  {
    int itemIndex = this.GetItemIndex(item);
    switch (itemIndex)
    {
      case -1:
        return false;
      case 0:
        this.Take();
        break;
      default:
        this.RemoveAt(itemIndex + 1, -1);
        break;
    }
    return true;
  }

  public int Count { get; private set; }

  public bool IsReadOnly => false;

  private void RemoveAt(int index, int shift)
  {
    int i = index + shift;
    --this.Count;
    this._heap.Swap<T>(i, this.Count);
    this._heap[this.Count] = default (T);
    int index1 = index / 2 + shift;
    if (this._comparer.GreaterOrEqual<T>(this._heap[i], this._heap[index1]))
      this._heap.Sift<T>(index, this._comparer, shift);
    else
      this._heap.Sink<T>(index, this.Count, this._comparer, shift);
  }

  private int GetItemIndex(T item)
  {
    for (int itemIndex = 0; itemIndex < this.Count; ++itemIndex)
    {
      if (this._comparer.Compare(this._heap[itemIndex], item) == 0)
        return itemIndex;
    }
    return -1;
  }

  private void GrowCapacity()
  {
    int newSize = this.Capacity * 2;
    Array.Resize<T>(ref this._heap, newSize);
    this._shrinkBound = newSize / 4;
  }

  private void ShrinkCapacity()
  {
    int newSize = this.Capacity / 2;
    Array.Resize<T>(ref this._heap, newSize);
    this._shrinkBound = newSize / 4;
  }
}
