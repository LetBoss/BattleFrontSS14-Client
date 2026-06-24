// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.RayHit
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Physics.Systems;

public record struct RayHit(EntityUid Entity, Vector2 LocalNormal, float Fraction)
{
  public readonly EntityUid Entity = Entity;
  public readonly Vector2 LocalNormal = LocalNormal;
  public readonly float Fraction = Fraction;
  public Vector2 Point = new Vector2();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.LocalNormal)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Fraction)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Point);
  }

  [CompilerGenerated]
  public readonly bool Equals(RayHit other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<Vector2>.Default.Equals(this.LocalNormal, other.LocalNormal) && EqualityComparer<float>.Default.Equals(this.Fraction, other.Fraction) && EqualityComparer<Vector2>.Default.Equals(this.Point, other.Point);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out EntityUid Entity,
    out Vector2 LocalNormal,
    out float Fraction)
  {
    Entity = this.Entity;
    LocalNormal = this.LocalNormal;
    Fraction = this.Fraction;
  }
}
