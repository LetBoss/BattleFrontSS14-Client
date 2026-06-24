// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.UniqueIndex`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Utility;

public struct UniqueIndex<TKey, TValue> : 
  IUniqueIndex<TKey, TValue>,
  IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>,
  IEnumerable
  where TKey : notnull
{
  private ImmutableDictionary<TKey, HashSet<TValue>>? _index;

  public int KeyCount
  {
    get
    {
      ImmutableDictionary<TKey, HashSet<TValue>> index = this._index;
      return index == null ? 0 : index.Count;
    }
  }

  public bool Add(TKey key, TValue value)
  {
    if (this._index == null)
    {
      HashSet<TValue> objSet = new HashSet<TValue>()
      {
        value
      };
      this._index = ImmutableDictionary.CreateRange<TKey, HashSet<TValue>>((IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>) new KeyValuePair<TKey, HashSet<TValue>>[1]
      {
        new KeyValuePair<TKey, HashSet<TValue>>(key, objSet)
      });
      return true;
    }
    HashSet<TValue> objSet1;
    if (this._index.TryGetValue(key, out objSet1))
      return objSet1.Add(value);
    this._index = this._index.Add(key, new HashSet<TValue>()
    {
      value
    });
    return true;
  }

  public int AddRange(TKey key, IEnumerable<TValue> values)
  {
    if (this._index == null)
    {
      HashSet<TValue> objSet = new HashSet<TValue>(values);
      this._index = ImmutableDictionary.CreateRange<TKey, HashSet<TValue>>((IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>) new KeyValuePair<TKey, HashSet<TValue>>[1]
      {
        new KeyValuePair<TKey, HashSet<TValue>>(key, objSet)
      });
      return objSet.Count;
    }
    HashSet<TValue> objSet1;
    if (this._index.TryGetValue(key, out objSet1))
    {
      int count = objSet1.Count;
      objSet1.UnionWith(values);
      return objSet1.Count - count;
    }
    HashSet<TValue> objSet2;
    this._index = this._index.Add(key, objSet2 = new HashSet<TValue>(values));
    return objSet2.Count;
  }

  public bool Remove(TKey key)
  {
    if (this._index == null)
      return false;
    ImmutableDictionary<TKey, HashSet<TValue>> immutableDictionary = this._index.Remove(key);
    if (this._index != immutableDictionary)
      return false;
    this._index = immutableDictionary;
    return true;
  }

  public bool Remove(TKey key, TValue value)
  {
    HashSet<TValue> objSet;
    return this._index != null && this._index.TryGetValue(key, out objSet) && objSet.Remove(value);
  }

  public int RemoveRange(TKey key, IEnumerable<TValue> values)
  {
    HashSet<TValue> objSet;
    if (this._index == null || !this._index.TryGetValue(key, out objSet))
      return 0;
    int count1 = objSet.Count;
    objSet.ExceptWith(values);
    int count2 = objSet.Count;
    return count1 - count2;
  }

  public bool Replace(TKey key, TValue oldValue, TValue newValue)
  {
    HashSet<TValue> objSet;
    return this._index != null && this._index.TryGetValue(key, out objSet) && objSet.Remove(oldValue) && objSet.Add(newValue);
  }

  public void Touch(TKey key)
  {
    if (this._index == null)
      this._index = ImmutableDictionary<TKey, HashSet<TValue>>.Empty;
    if (this._index.ContainsKey(key))
      return;
    this._index = this._index.Add(key, new HashSet<TValue>());
  }

  public void Initialize(IEnumerable<TKey> keys)
  {
    this.Initialize(keys.Select<TKey, KeyValuePair<TKey, HashSet<TValue>>>((Func<TKey, KeyValuePair<TKey, HashSet<TValue>>>) (k => new KeyValuePair<TKey, HashSet<TValue>>(k, new HashSet<TValue>()))));
  }

  public void Initialize(
    IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index)
  {
    this._index = this._index == null ? ImmutableDictionary.CreateRange<TKey, HashSet<TValue>>(index) : throw new InvalidOperationException("Already initialized.");
  }

  public ISet<TValue> this[TKey key]
  {
    get
    {
      if (this._index == null)
      {
        this._index = ImmutableDictionary<TKey, HashSet<TValue>>.Empty;
      }
      else
      {
        HashSet<TValue> objSet;
        if (this._index.TryGetValue(key, out objSet))
          return (ISet<TValue>) objSet;
      }
      HashSet<TValue> objSet1;
      this._index = this._index.Add(key, objSet1 = new HashSet<TValue>());
      return (ISet<TValue>) objSet1;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
  {
    return this._index != null ? (IEnumerator<KeyValuePair<TKey, HashSet<TValue>>>) this._index.GetEnumerator() : Enumerable.Empty<KeyValuePair<TKey, HashSet<TValue>>>().GetEnumerator();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
