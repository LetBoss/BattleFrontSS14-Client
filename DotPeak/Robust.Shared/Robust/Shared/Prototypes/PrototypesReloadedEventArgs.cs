// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypesReloadedEventArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

public sealed record PrototypesReloadedEventArgs(
  HashSet<Type> Modified,
  IReadOnlyDictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet> ByType,
  IReadOnlyDictionary<Type, HashSet<string>>? Removed = null)
{
  public bool WasModified<T>() where T : IPrototype => this.Modified.Contains(typeof (T));

  public bool TryGetModified<T>([NotNullWhen(true)] out HashSet<string>? modified) where T : IPrototype
  {
    modified = (HashSet<string>) null;
    if (!this.WasModified<T>())
      return false;
    modified = new HashSet<string>();
    PrototypesReloadedEventArgs.PrototypeChangeSet prototypeChangeSet;
    if (this.ByType.TryGetValue(typeof (T), out prototypeChangeSet))
      modified.UnionWith(prototypeChangeSet.Modified.Keys);
    HashSet<string> other;
    if (this.Removed != null && this.Removed.TryGetValue(typeof (T), out other))
      modified.UnionWith((IEnumerable<string>) other);
    return true;
  }

  public sealed record PrototypeChangeSet(IReadOnlyDictionary<string, IPrototype> Modified);
}
