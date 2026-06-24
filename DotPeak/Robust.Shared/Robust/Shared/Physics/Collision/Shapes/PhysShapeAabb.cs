// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.PhysShapeAabb
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision.Shapes;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class PhysShapeAabb : 
  IPhysShape,
  IEquatable<IPhysShape>,
  IEquatable<PhysShapeAabb>,
  ISerializationGenerated<PhysShapeAabb>,
  ISerializationGenerated
{
  private float _radius;
  [DataField("bounds", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  private Box2 _localBounds = Box2.UnitCentered;

  public int ChildCount => 1;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Radius
  {
    get => this._radius;
    set
    {
      if (MathHelper.CloseToPercent(this._radius, value, 1E-05))
        return;
      this._radius = value;
    }
  }

  internal Vector2 Centroid => Vector2.Zero;

  public ShapeType ShapeType => ShapeType.Unknown;

  public Box2 LocalBounds => this._localBounds;

  public PhysShapeAabb(float radius) => this._radius = radius;

  public PhysShapeAabb() => this._radius = 0.01f;

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    Box2Rotated box2Rotated = new Box2Rotated(((Box2) ref this._localBounds).Translated(transform.Position), Angle.op_Implicit(transform.Quaternion2D.Angle), transform.Position);
    Box2 box2 = ((Box2Rotated) ref box2Rotated).CalcBoundingBox();
    return ((Box2) ref box2).Enlarged(this._radius);
  }

  internal List<Vector2> GetVertices()
  {
    return new List<Vector2>()
    {
      ((Box2) ref this._localBounds).BottomRight,
      this._localBounds.TopRight,
      ((Box2) ref this._localBounds).TopLeft,
      this._localBounds.BottomLeft
    };
  }

  public bool Equals(IPhysShape? other)
  {
    return other is PhysShapeAabb physShapeAabb && ((Box2) ref this._localBounds).EqualsApprox(physShapeAabb._localBounds);
  }

  public bool Equals(PhysShapeAabb? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this._radius.Equals(other._radius) && ((Box2) ref this._localBounds).Equals(other._localBounds);
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is PhysShapeAabb other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<float, Box2>(this._radius, this._localBounds);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PhysShapeAabb target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PhysShapeAabb>(this, ref target, hookCtx, false, context))
      return;
    Box2 target1 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this._localBounds, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<Box2>(this._localBounds, hookCtx, context);
    target._localBounds = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PhysShapeAabb target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PhysShapeAabb target1 = (PhysShapeAabb) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PhysShapeAabb Instantiate() => new PhysShapeAabb();
}
