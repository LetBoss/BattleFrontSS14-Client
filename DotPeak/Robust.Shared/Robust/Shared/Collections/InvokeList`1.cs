// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Collections.InvokeList`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Collections;

internal struct InvokeList<T>
{
  private InvokeList<
  #nullable disable
  T>.Entry[]
  #nullable enable
  ? _entries;

  public int Count
  {
    get
    {
      InvokeList<T>.Entry[] entries = this._entries;
      return entries == null ? 0 : entries.Length;
    }
  }

  public void AddInPlace(T value, object equality) => this = this.Add(value, equality);

  public readonly InvokeList<T> Add(T value, object equality)
  {
    if (this._entries == null)
    {
      InvokeList<T> invokeList = new InvokeList<T>();
      // ISSUE: explicit reference operation
      (^ref invokeList)._entries = new InvokeList<T>.Entry[1]
      {
        new InvokeList<T>.Entry()
        {
          Value = value,
          Equality = equality
        }
      };
      return invokeList;
    }
    InvokeList<T>.Entry[] entries = this._entries;
    Array.Resize<InvokeList<T>.Entry>(ref entries, entries.Length + 1);
    InvokeList<T>.Entry[] entryArray = entries;
    entryArray[entryArray.Length - 1] = new InvokeList<T>.Entry()
    {
      Value = value,
      Equality = equality
    };
    return new InvokeList<T>() { _entries = entries };
  }

  public void RemoveInPlace(object equality) => this = this.Remove(equality);

  public readonly InvokeList<T> Remove(object equality)
  {
    if (this._entries == null)
      return this;
    int num = -1;
    for (int index = 0; index < this._entries.Length; ++index)
    {
      InvokeList<T>.Entry entry = this._entries[index];
      if (equality.Equals(entry.Equality))
      {
        num = index;
        break;
      }
    }
    if (num < 0)
      return this;
    if (this._entries.Length == 1)
      return new InvokeList<T>();
    InvokeList<T>.Entry[] entryArray = new InvokeList<T>.Entry[this._entries.Length - 1];
    int index1 = 0;
    for (int index2 = 0; index2 < entryArray.Length; ++index2)
    {
      if (index1 == num)
        ++index1;
      entryArray[index2] = this._entries[index1];
      ++index1;
    }
    return new InvokeList<T>() { _entries = entryArray };
  }

  public ReadOnlySpan<InvokeList<
  #nullable disable
  T>.Entry> Entries => (ReadOnlySpan<InvokeList<T>.Entry>) this._entries;

  public struct Entry
  {
    public 
    #nullable enable
    T? Value;
    public object? Equality;
  }
}
