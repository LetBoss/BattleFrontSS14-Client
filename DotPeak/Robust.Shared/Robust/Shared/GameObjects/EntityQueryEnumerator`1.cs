// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityQueryEnumerator`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct EntityQueryEnumerator<TComp1> : IDisposable where TComp1 : IComponent
{
  private Dictionary<EntityUid, IComponent>.Enumerator _traitDict;
  private readonly EntityQuery<MetaDataComponent> _metaQuery;

  public EntityQueryEnumerator(
    Dictionary<EntityUid, IComponent> traitDict,
    EntityQuery<MetaDataComponent> metaQuery)
  {
    this._traitDict = traitDict.GetEnumerator();
    this._metaQuery = metaQuery;
  }

  public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1)
  {
    while (this._traitDict.MoveNext())
    {
      KeyValuePair<EntityUid, IComponent> current = this._traitDict.Current;
      MetaDataComponent component;
      if (!current.Value.Deleted && this._metaQuery.TryGetComponentInternal(current.Key, out component) && !component.EntityPaused)
      {
        uid = current.Key;
        comp1 = (TComp1) current.Value;
        return true;
      }
    }
    uid = new EntityUid();
    comp1 = default (TComp1);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool MoveNext([NotNullWhen(true)] out TComp1? comp1)
  {
    return this.MoveNext(out EntityUid _, out comp1);
  }

  public void Dispose() => this._traitDict.Dispose();
}
