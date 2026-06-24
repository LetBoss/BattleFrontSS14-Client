// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.IUniqueIndex`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

internal interface IUniqueIndex<TKey, TValue> : 
  IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>,
  IEnumerable
  where TKey : notnull
{
  int KeyCount { get; }

  bool Add(TKey key, TValue value);

  int AddRange(TKey key, IEnumerable<TValue> values);

  bool Remove(TKey key);

  bool Remove(TKey key, TValue value);

  int RemoveRange(TKey key, IEnumerable<TValue> values);

  bool Replace(TKey key, TValue oldValue, TValue newValue);

  void Touch(TKey key);

  void Initialize(IEnumerable<TKey> keys);

  void Initialize(
    IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index);
}
