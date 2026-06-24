// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ComponentQueryEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct ComponentQueryEnumerator : IDisposable
{
  private Dictionary<EntityUid, IComponent>.Enumerator _traitDict;

  public ComponentQueryEnumerator(Dictionary<EntityUid, IComponent> traitDict)
  {
    this._traitDict = traitDict.GetEnumerator();
  }

  public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out IComponent? comp1)
  {
    while (this._traitDict.MoveNext())
    {
      KeyValuePair<EntityUid, IComponent> current = this._traitDict.Current;
      if (!current.Value.Deleted)
      {
        uid = current.Key;
        comp1 = current.Value;
        return true;
      }
    }
    uid = new EntityUid();
    comp1 = (IComponent) null;
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool MoveNext([NotNullWhen(true)] out IComponent? comp1)
  {
    return this.MoveNext(out EntityUid _, out comp1);
  }

  public void Dispose() => this._traitDict.Dispose();
}
