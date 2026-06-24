// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Enumerators.AnchoredEntitiesEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Map.Enumerators;

public struct AnchoredEntitiesEnumerator : IDisposable
{
  private static readonly List<EntityUid> Dummy = new List<EntityUid>();
  public static readonly AnchoredEntitiesEnumerator Empty = new AnchoredEntitiesEnumerator(AnchoredEntitiesEnumerator.Dummy.GetEnumerator());
  private List<EntityUid>.Enumerator _enumerator;

  internal AnchoredEntitiesEnumerator(List<EntityUid>.Enumerator enumerator)
  {
    this._enumerator = enumerator;
  }

  public bool MoveNext([NotNullWhen(true)] out EntityUid? uid)
  {
    if (!this._enumerator.MoveNext())
    {
      uid = new EntityUid?();
      return false;
    }
    uid = new EntityUid?(this._enumerator.Current);
    return true;
  }

  public void Dispose() => this._enumerator.Dispose();
}
