// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Collections.OverflowDictionary`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Collections;

public sealed class OverflowDictionary<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable,
  IDisposable
  where TKey : notnull
{
  private TKey[] _insertionQueue;
  private int _currentIndex;
  private IDictionary<TKey, TValue> _dict;
  private Action<TValue>? _valueDisposer;

  public int Capacity => this._insertionQueue.Length;

  public OverflowDictionary(int capacity, Action<TValue>? valueDisposer = null)
  {
    if (capacity <= 0)
      throw new InvalidOperationException("Cannot create an OverflowDictionary with a capacity of less than 1.");
    this._valueDisposer = valueDisposer;
    this._dict = (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>(capacity);
    this._insertionQueue = new TKey[capacity];
  }

  private int GetArrayStartIndex() => this._currentIndex % this.Capacity;

  public void Clear()
  {
    this._dict.Clear();
    Array.Clear((Array) this._insertionQueue);
  }

  public void Add(TKey key, TValue value)
  {
    if (this._dict.ContainsKey(key))
      throw new InvalidOperationException("Tried inserting duplicate key.");
    if (this.Count == this.Capacity)
    {
      int arrayStartIndex = this.GetArrayStartIndex();
      TKey insertion = this._insertionQueue[arrayStartIndex];
      Array.Clear((Array) this._insertionQueue, arrayStartIndex, 1);
      Action<TValue> valueDisposer = this._valueDisposer;
      if (valueDisposer != null)
        valueDisposer(this._dict[insertion]);
      this._dict.Remove(insertion);
    }
    this._dict.Add(key, value);
    this._insertionQueue[this._currentIndex++] = key;
    if (this._currentIndex != this.Capacity)
      return;
    this._currentIndex = 0;
  }

  public bool Add(TKey key, TValue value, [NotNullWhen(true)] out (TKey Key, TValue Value)? old)
  {
    if (this._dict.ContainsKey(key))
      throw new InvalidOperationException("Tried inserting duplicate key.");
    if (this.Count == this.Capacity)
    {
      int arrayStartIndex = this.GetArrayStartIndex();
      TKey insertion = this._insertionQueue[arrayStartIndex];
      TValue obj;
      this._dict.Remove<TKey, TValue>(insertion, out obj);
      Array.Clear((Array) this._insertionQueue, arrayStartIndex, 1);
      Action<TValue> valueDisposer = this._valueDisposer;
      if (valueDisposer != null)
        valueDisposer(obj);
      old = new (TKey, TValue)?((insertion, obj));
    }
    else
      old = new (TKey, TValue)?();
    this._dict.Add(key, value);
    this._insertionQueue[this._currentIndex++] = key;
    if (this._currentIndex == this.Capacity)
      this._currentIndex = 0;
    return old.HasValue;
  }

  public bool Remove(TKey key)
  {
    throw new NotImplementedException("Removing from an Overflowdictionary is not yet supported");
  }

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this._dict.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

  public bool Contains(KeyValuePair<TKey, TValue> item) => this._dict.Contains(item);

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    this._dict.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

  public ICollection<TKey> Keys => this._dict.Keys;

  public ICollection<TValue> Values => this._dict.Values;

  public int Count => this._dict.Count;

  public bool IsReadOnly => false;

  public bool ContainsKey(TKey key) => this._dict.ContainsKey(key);

  public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
  {
    return this._dict.TryGetValue(key, out value);
  }

  public TValue this[TKey key]
  {
    get => this._dict[key];
    set => this._dict[key] = value;
  }

  public void Dispose()
  {
    foreach (TValue obj in (IEnumerable<TValue>) this._dict.Values)
    {
      Action<TValue> valueDisposer = this._valueDisposer;
      if (valueDisposer != null)
        valueDisposer(obj);
    }
  }
}
