// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TransformChildrenEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Robust.Shared.GameObjects;

public struct TransformChildrenEnumerator(HashSet<EntityUid>.Enumerator children) : IDisposable
{
  private HashSet<EntityUid>.Enumerator _children = children;

  public bool MoveNext(out EntityUid child)
  {
    if (!this._children.MoveNext())
    {
      child = new EntityUid();
      return false;
    }
    child = this._children.Current;
    return true;
  }

  public void Dispose() => this._children.Dispose();
}
