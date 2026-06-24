// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.PrototypeFlags`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class PrototypeFlags<T> : IReadOnlyPrototypeFlags<T>, IEnumerable<string>, IEnumerable where T : class, IPrototype
{
  private readonly HashSet<string> _flags;

  public PrototypeFlags() => this._flags = new HashSet<string>();

  public PrototypeFlags(params string[] flags)
  {
    this._flags = new HashSet<string>((IEnumerable<string>) flags);
  }

  public PrototypeFlags(IEnumerable<string> flags) => this._flags = new HashSet<string>(flags);

  public int Count => this._flags.Count;

  public bool Add(string flag, IPrototypeManager prototypeManager)
  {
    return prototypeManager.HasIndex<T>(flag) && this._flags.Add(flag);
  }

  internal void UnionWith(PrototypeFlags<T> flags)
  {
    this._flags.UnionWith((IEnumerable<string>) flags._flags);
  }

  public bool Contains(string flag) => this._flags.Contains(flag);

  public bool ContainsAll(IEnumerable<string> flags)
  {
    foreach (string flag in flags)
    {
      if (!this.Contains(flag))
        return false;
    }
    return true;
  }

  public bool ContainsAll(params string[] flags) => this.ContainsAll((IEnumerable<string>) flags);

  public bool ContainsAny(IEnumerable<string> flags)
  {
    foreach (string flag in flags)
    {
      if (this.Contains(flag))
        return true;
    }
    return false;
  }

  public bool ContainsAny(params string[] flags) => this.ContainsAny((IEnumerable<string>) flags);

  public bool Remove(string flag) => this._flags.Remove(flag);

  public void Clear() => this._flags.Clear();

  public IEnumerable<T> GetPrototypes(IPrototypeManager prototypeManager)
  {
    foreach (string flag in this._flags)
      yield return prototypeManager.Index<T>(flag);
  }

  public IEnumerator<string> GetEnumerator() => (IEnumerator<string>) this._flags.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
