// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.CollisionRay
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

[Serializable]
public readonly struct CollisionRay(Vector2 position, Vector2 direction, int collisionMask) : 
  IEquatable<CollisionRay>
{
  private readonly Ray _ray = new Ray(position, direction);
  private readonly int _collisionMask = collisionMask;

  public Vector2 Position => this._ray.Position;

  public Vector2 Direction => this._ray.Direction;

  public int CollisionMask => this._collisionMask;

  public bool Intersects(Box2 box, out float distance, out Vector2 hitPos)
  {
    return this._ray.Intersects(box, out distance, out hitPos);
  }

  public bool Equals(CollisionRay other)
  {
    return this.Position.Equals(other.Position) && this.Direction.Equals(other.Direction);
  }

  public override bool Equals(object? obj)
  {
    return obj != null && obj is CollisionRay other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    Vector2 vector2 = this.Position;
    int num = vector2.GetHashCode() * 397;
    vector2 = this.Direction;
    int hashCode = vector2.GetHashCode();
    return num ^ hashCode;
  }

  public static bool operator ==(CollisionRay a, CollisionRay b) => a.Equals(b);

  public static bool operator !=(CollisionRay a, CollisionRay b) => !(a == b);

  public static implicit operator Ray(CollisionRay a) => a._ray;
}
