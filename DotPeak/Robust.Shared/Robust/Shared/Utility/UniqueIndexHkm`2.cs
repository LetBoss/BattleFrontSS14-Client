// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.UniqueIndexHkm`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Utility;

public struct UniqueIndexHkm<TKey, TValue>(int capacity) : 
  IUniqueIndex<TKey, TValue>,
  IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>,
  IEnumerable
  where TKey : notnull
{
  private readonly Dictionary<TKey, HashSet<TValue>> _index = new Dictionary<TKey, HashSet<TValue>>(capacity);

  public int KeyCount => this._index.Count;

  private void InitializedCheck()
  {
    if (this._index == null)
      throw new NotSupportedException("UniqueIndexHkm instances must use the non-default constructor.");
  }

  public bool Add(TKey key, TValue value)
  {
    this.InitializedCheck();
    return this._index.GetOrNew<TKey, HashSet<TValue>>(key).Add(value);
  }

  public int AddRange(TKey key, IEnumerable<TValue> values)
  {
    this.InitializedCheck();
    HashSet<TValue> objSet1;
    if (this._index.TryGetValue(key, out objSet1))
    {
      int count = objSet1.Count;
      objSet1.UnionWith(values);
      return objSet1.Count - count;
    }
    HashSet<TValue> objSet2;
    this._index.Add(key, objSet2 = new HashSet<TValue>(values));
    return objSet2.Count;
  }

  public bool Remove(TKey key)
  {
    this.InitializedCheck();
    return this._index.Remove(key);
  }

  public bool Remove(TKey key, TValue value)
  {
    this.InitializedCheck();
    HashSet<TValue> objSet;
    return this._index.Count != 0 && this._index.TryGetValue(key, out objSet) && objSet.Remove(value);
  }

  public int RemoveRange(TKey key, IEnumerable<TValue> values)
  {
    this.InitializedCheck();
    HashSet<TValue> objSet;
    if (!this._index.TryGetValue(key, out objSet))
      return 0;
    int count1 = objSet.Count;
    objSet.ExceptWith(values);
    int count2 = objSet.Count;
    return count1 - count2;
  }

  public bool Replace(TKey key, TValue oldValue, TValue newValue)
  {
    this.InitializedCheck();
    HashSet<TValue> objSet;
    return this._index.Count != 0 && this._index.TryGetValue(key, out objSet) && objSet.Remove(oldValue) && objSet.Add(newValue);
  }

  public void Touch(TKey key)
  {
    this.InitializedCheck();
    if (this._index.ContainsKey(key))
      return;
    this._index.Add(key, new HashSet<TValue>());
  }

  public void Initialize(IEnumerable<TKey> keys)
  {
    this.Initialize(keys.Select<TKey, KeyValuePair<TKey, HashSet<TValue>>>((Func<TKey, KeyValuePair<TKey, HashSet<TValue>>>) (k => new KeyValuePair<TKey, HashSet<TValue>>(k, new HashSet<TValue>()))));
  }

  public void Initialize(
    IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index)
  {
    this.InitializedCheck();
    if (this._index.Count != 0)
      throw new InvalidOperationException("Already initialized.");
    foreach ((TKey key, HashSet<TValue> objSet) in index)
      this._index.Add(key, objSet);
  }

  public HashSet<TValue> this[TKey key]
  {
    get
    {
      this.InitializedCheck();
      HashSet<TValue> objSet1;
      if (this._index.TryGetValue(key, out objSet1))
        return objSet1;
      HashSet<TValue> objSet2;
      this._index.Add(key, objSet2 = new HashSet<TValue>());
      return objSet2;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
  {
    this.InitializedCheck();
    return (IEnumerator<KeyValuePair<TKey, HashSet<TValue>>>) this._index.GetEnumerator();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Clear()
  {
    this.InitializedCheck();
    this._index.Clear();
  }
}
