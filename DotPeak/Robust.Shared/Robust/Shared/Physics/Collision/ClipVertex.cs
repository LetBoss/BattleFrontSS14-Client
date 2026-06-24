// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.ClipVertex
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Physics.Collision;

internal record struct ClipVertex
{
  public ContactID ID;
  public Vector2 V;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<ContactID>.Default.GetHashCode(this.ID) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.V);
  }

  [CompilerGenerated]
  public readonly bool Equals(ClipVertex other)
  {
    return EqualityComparer<ContactID>.Default.Equals(this.ID, other.ID) && EqualityComparer<Vector2>.Default.Equals(this.V, other.V);
  }
}
