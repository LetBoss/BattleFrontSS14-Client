// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.AllEntityQueryEnumerator`3
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct AllEntityQueryEnumerator<TComp1, TComp2, TComp3> : IDisposable
  where TComp1 : IComponent
  where TComp2 : IComponent
  where TComp3 : IComponent
{
  private Dictionary<EntityUid, IComponent>.Enumerator _traitDict;
  private readonly Dictionary<EntityUid, IComponent> _traitDict2;
  private readonly Dictionary<EntityUid, IComponent> _traitDict3;

  public AllEntityQueryEnumerator(
    Dictionary<EntityUid, IComponent> traitDict,
    Dictionary<EntityUid, IComponent> traitDict2,
    Dictionary<EntityUid, IComponent> traitDict3)
  {
    this._traitDict = traitDict.GetEnumerator();
    this._traitDict2 = traitDict2;
    this._traitDict3 = traitDict3;
  }

  public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3)
  {
    while (this._traitDict.MoveNext())
    {
      KeyValuePair<EntityUid, IComponent> current = this._traitDict.Current;
      IComponent component1;
      IComponent component2;
      if (!current.Value.Deleted && this._traitDict2.TryGetValue(current.Key, out component1) && !component1.Deleted && this._traitDict3.TryGetValue(current.Key, out component2) && !component2.Deleted)
      {
        uid = current.Key;
        comp1 = (TComp1) current.Value;
        comp2 = (TComp2) component1;
        comp3 = (TComp3) component2;
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
