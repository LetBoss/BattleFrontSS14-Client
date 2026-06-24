// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.PhysShapeCircle
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision.Shapes;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class PhysShapeCircle : 
  IPhysShape,
  IEquatable<IPhysShape>,
  IEquatable<PhysShapeCircle>,
  ISerializationGenerated<PhysShapeCircle>,
  ISerializationGenerated
{
  private const float DefaultRadius = 0.5f;
  [DataField("position", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public Vector2 Position;

  public int ChildCount => 1;

  public ShapeType ShapeType => ShapeType.Circle;

  [DataField("radius", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Radius { get; set; } = 0.5f;

  public PhysShapeCircle()
  {
  }

  public PhysShapeCircle(float radius)
  {
    this.Radius = radius;
    this.Position = Vector2.Zero;
  }

  public PhysShapeCircle(float radius, Vector2 position)
  {
    this.Radius = radius;
    this.Position = position;
  }

  public float CalculateArea() => 3.14159274f * this.Radius * this.Radius;

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    Vector2 vector2 = transform.Position + Transform.Mul(in transform.Quaternion2D, in this.Position);
    return new Box2(vector2.X - this.Radius, vector2.Y - this.Radius, vector2.X + this.Radius, vector2.Y + this.Radius);
  }

  public Box2 CalcLocalBounds()
  {
    return new Box2(this.Position.X - this.Radius, this.Position.Y - this.Radius, this.Position.X + this.Radius, this.Position.Y + this.Radius);
  }

  public bool Equals(IPhysShape? other)
  {
    return other is PhysShapeCircle physShapeCircle && physShapeCircle.Equals(this);
  }

  public bool Equals(PhysShapeCircle? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return MathHelper.CloseTo(this.Radius, other.Radius, 1E-07f) && Vector2Helpers.EqualsApprox(this.Position, other.Position);
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is PhysShapeCircle other && this.Equals(other);
  }

  public override int GetHashCode() => HashCode.Combine<float, Vector2>(this.Radius, this.Position);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PhysShapeCircle target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PhysShapeCircle>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target1, hookCtx, false, context))
      target1 = this.Radius;
    target.Radius = target1;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Position, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.Position, hookCtx, context);
    target.Position = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PhysShapeCircle target,
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
    PhysShapeCircle target1 = (PhysShapeCircle) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PhysShapeCircle Instantiate() => new PhysShapeCircle();
}
