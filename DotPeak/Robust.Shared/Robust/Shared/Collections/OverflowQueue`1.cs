// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Collections.OverflowQueue`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Collections;

public sealed class OverflowQueue<T>
{
  private readonly T[] _queue;
  private int _currentIdx;
  private int _length;

  public int Size => this._queue.Length;

  public OverflowQueue(int size) => this._queue = new T[size];

  public void Enqueue(T item)
  {
    this._queue[this._currentIdx++] = item;
    if (this._length < this.Size)
      ++this._length;
    if (this._currentIdx != this.Size)
      return;
    this._currentIdx = 0;
  }

  public T Dequeue()
  {
    T obj;
    if (!this.TryDequeue(out obj))
      throw new InvalidOperationException("OverflowQueue has no more items to dequeue.");
    return obj;
  }

  public bool TryDequeue([NotNullWhen(true)] out T? item)
  {
    if (this._length == 0)
    {
      item = default (T);
      return false;
    }
    item = this._queue[this.GetCurrentIndex()];
    --this._length;
    return true;
  }

  private int GetCurrentIndex()
  {
    int num = this._currentIdx - this._length;
    return num < 0 ? num + this.Size : num;
  }

  public T Peek()
  {
    if (this._length == 0)
      throw new InvalidOperationException("OverflowQueue has no more items to dequeue.");
    return this._queue[this.GetCurrentIndex()];
  }

  public bool Contains(T item)
  {
    for (int index1 = 0; index1 < this._length; ++index1)
    {
      int index2 = this._currentIdx + index1;
      if (index2 >= this._length)
        index2 -= this._length;
      if (EqualityComparer<T>.Default.Equals(item, this._queue[index2]))
        return true;
    }
    return false;
  }

  public T[] ToArray()
  {
    if (this._length == 0)
      return Array.Empty<T>();
    T[] destinationArray = new T[this._length];
    int sourceIndex = this._currentIdx - this._length;
    if (sourceIndex < 0)
    {
      Array.Copy((Array) this._queue, sourceIndex + this.Size, (Array) destinationArray, 0, -1 * sourceIndex);
      Array.Copy((Array) this._queue, 0, (Array) destinationArray, -1 * sourceIndex, sourceIndex + this.Size);
    }
    else
      Array.Copy((Array) this._queue, sourceIndex, (Array) destinationArray, 0, this._length);
    return destinationArray;
  }
}
