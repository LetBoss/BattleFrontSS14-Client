// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.EdgeShape
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
public sealed class EdgeShape : 
  IPhysShape,
  IEquatable<IPhysShape>,
  IEquatable<EdgeShape>,
  ISerializationGenerated<EdgeShape>,
  ISerializationGenerated
{
  [DataField("vertex1", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal Vector2 Vertex1;
  [DataField("vertex2", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal Vector2 Vertex2;
  [DataField("vertex0", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal Vector2 Vertex0;
  [DataField("vertex3", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal Vector2 Vertex3;
  [DataField("oneSided", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool OneSided;

  public int ChildCount => 1;

  public ShapeType ShapeType => ShapeType.Edge;

  [DataField("radius", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Radius { get; set; } = 0.01f;

  public EdgeShape()
  {
  }

  public EdgeShape(Vector2 v1, Vector2 v2) => this.SetTwoSided(v1, v2);

  public void SetOneSided(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
  {
    this.Vertex0 = v0;
    this.Vertex1 = v1;
    this.Vertex2 = v2;
    this.Vertex3 = v3;
    this.OneSided = true;
  }

  public void SetTwoSided(Vector2 start, Vector2 end)
  {
    this.Vertex1 = start;
    this.Vertex2 = end;
    this.OneSided = false;
  }

  public bool Equals(IPhysShape? other)
  {
    return other is EdgeShape edgeShape && this.OneSided == edgeShape.OneSided && this.Vertex0.Equals(edgeShape.Vertex0) && this.Vertex1.Equals(edgeShape.Vertex1) && this.Vertex2.Equals(edgeShape.Vertex2) && this.Vertex3.Equals(edgeShape.Vertex3);
  }

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    Vector2 vector2_1 = Transform.Mul(in transform, in this.Vertex1);
    Vector2 vector2_2 = Transform.Mul(in transform, in this.Vertex2);
    Vector2 vector2_3 = Vector2.Min(vector2_1, vector2_2);
    Vector2 vector2_4 = Vector2.Max(vector2_1, vector2_2);
    Vector2 vector2_5 = new Vector2(this.Radius, this.Radius);
    return new Box2(vector2_3 - vector2_5, vector2_4 + vector2_5);
  }

  public float CalculateArea() => 0.0f;

  public bool Equals(EdgeShape? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.OneSided == other.OneSided && this.Vertex1.Equals(other.Vertex1) && this.Vertex2.Equals(other.Vertex2) && this.Vertex0.Equals(other.Vertex0) && this.Vertex3.Equals(other.Vertex3) && this.Radius.Equals(other.Radius);
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is EdgeShape other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<bool, Vector2, Vector2, Vector2, Vector2, float>(this.OneSided, this.Vertex1, this.Vertex2, this.Vertex0, this.Vertex3, this.Radius);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EdgeShape target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EdgeShape>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target1 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Vertex1, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<Vector2>(this.Vertex1, hookCtx, context);
    target.Vertex1 = target1;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Vertex2, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.Vertex2, hookCtx, context);
    target.Vertex2 = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Vertex0, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.Vertex0, hookCtx, context);
    target.Vertex0 = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Vertex3, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.Vertex3, hookCtx, context);
    target.Vertex3 = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.OneSided, ref target5, hookCtx, false, context))
      target5 = this.OneSided;
    target.OneSided = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target6, hookCtx, false, context))
      target6 = this.Radius;
    target.Radius = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EdgeShape target,
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
    EdgeShape target1 = (EdgeShape) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public EdgeShape Instantiate() => new EdgeShape();
}
