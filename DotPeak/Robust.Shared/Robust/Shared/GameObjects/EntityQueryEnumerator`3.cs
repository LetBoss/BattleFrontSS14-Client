// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityQueryEnumerator`3
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct EntityQueryEnumerator<TComp1, TComp2, TComp3> : IDisposable
  where TComp1 : IComponent
  where TComp2 : IComponent
  where TComp3 : IComponent
{
  private Dictionary<EntityUid, IComponent>.Enumerator _traitDict;
  private readonly Dictionary<EntityUid, IComponent> _traitDict2;
  private readonly Dictionary<EntityUid, IComponent> _traitDict3;
  private readonly EntityQuery<MetaDataComponent> _metaQuery;

  public EntityQueryEnumerator(
    Dictionary<EntityUid, IComponent> traitDict,
    Dictionary<EntityUid, IComponent> traitDict2,
    Dictionary<EntityUid, IComponent> traitDict3,
    EntityQuery<MetaDataComponent> metaQuery)
  {
    this._traitDict = traitDict.GetEnumerator();
    this._traitDict2 = traitDict2;
    this._traitDict3 = traitDict3;
    this._metaQuery = metaQuery;
  }

  public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3)
  {
    while (this._traitDict.MoveNext())
    {
      KeyValuePair<EntityUid, IComponent> current = this._traitDict.Current;
      MetaDataComponent component1;
      IComponent component2;
      IComponent component3;
      if (!current.Value.Deleted && this._metaQuery.TryGetComponentInternal(current.Key, out component1) && !component1.EntityPaused && this._traitDict2.TryGetValue(current.Key, out component2) && !component2.Deleted && this._traitDict3.TryGetValue(current.Key, out component3) && !component3.Deleted)
      {
        uid = current.Key;
        comp1 = (TComp1) current.Value;
        comp2 = (TComp2) component2;
        comp3 = (TComp3) component3;
        return true;
      }
    }
    uid = new EntityUid();
    comp1 = default (TComp1);
    comp2 = default (TComp2);
    comp3 = default (TComp3);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool MoveNext([NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3)
  {
    return this.MoveNext(out EntityUid _, out comp1, out comp2, out comp3);
  }

  public void Dispose() => this._traitDict.Dispose();
}
