// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Fixture
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Dynamics;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class Fixture : 
  IEquatable<Fixture>,
  ISerializationHooks,
  ISerializationGenerated<Fixture>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [NonSerialized]
  public int ProxyCount;
  [NonSerialized]
  public EntityUid Owner;
  [Robust.Shared.ViewVariables.ViewVariables]
  [NonSerialized]
  public Dictionary<Fixture, Contact> Contacts = new Dictionary<Fixture, Contact>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("friction", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Friction = 0.4f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("restitution", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Restitution;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("hard", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool Hard = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("density", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Density = 1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("layer", false, 1, false, false, typeof (FlagSerializer<Robust.Shared.Physics.Dynamics.CollisionLayer>))]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public int CollisionLayer;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("mask", false, 1, false, false, typeof (FlagSerializer<Robust.Shared.Physics.Dynamics.CollisionMask>))]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public int CollisionMask;

  [field: NonSerialized]
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixtureProxy[] Proxies { get; set; } = Array.Empty<FixtureProxy>();

  [DataField("shape", false, 1, false, false, null)]
  public IPhysShape Shape { get; private set; } = (IPhysShape) new PhysShapeAabb();

  unsafe void ISerializationHooks.AfterDeserialization()
  {
    if (!(this.Shape is PhysShapeAabb shape))
      return;
    Box2 localBounds = shape.LocalBounds;
    PolygonShape polygonShape = new PolygonShape();
    ; // Unable to render the statement
    Span<Vector2> vertices = new Span<Vector2>((void*) pointer, 4);
    vertices[0] = localBounds.BottomLeft;
    vertices[1] = ((Box2) ref localBounds).BottomRight;
    vertices[2] = localBounds.TopRight;
    vertices[3] = ((Box2) ref localBounds).TopLeft;
    polygonShape.Set((ReadOnlySpan<Vector2>) vertices, 4);
    this.Shape = (IPhysShape) polygonShape;
  }

  internal Fixture(
    IPhysShape shape,
    int collisionLayer,
    int collisionMask,
    bool hard,
    float density = 1f,
    float friction = 0.4f,
    float restitution = 0.0f)
  {
    this.Shape = shape;
    this.CollisionLayer = collisionLayer;
    this.CollisionMask = collisionMask;
    this.Hard = hard;
    this.Density = density;
    this.Friction = friction;
    this.Restitution = restitution;
  }

  public Fixture()
  {
  }

  internal void CopyTo(Fixture fixture)
  {
    fixture.Shape = this.Shape;
    fixture.Friction = this.Friction;
    fixture.Restitution = this.Restitution;
    fixture.Hard = this.Hard;
    fixture.CollisionLayer = this.CollisionLayer;
    fixture.CollisionMask = this.CollisionMask;
    fixture.Density = this.Density;
  }

  public bool Equivalent(Fixture other)
  {
    return this.Hard == other.Hard && this.CollisionLayer == other.CollisionLayer && this.CollisionMask == other.CollisionMask && this.Shape.Equals(other.Shape) && MathHelper.CloseTo(this.Density, other.Density, 1E-07f);
  }

  public bool Equals(Fixture? other)
  {
    return other != null && this.Equivalent(other) && this.Owner == other.Owner;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Fixture target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Fixture>(this, ref target, hookCtx, true, context))
      return;
    IPhysShape target1 = (IPhysShape) null;
    if (this.Shape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.Shape, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<IPhysShape>(this.Shape, hookCtx, context);
    target.Shape = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Friction, ref target2, hookCtx, false, context))
      target2 = this.Friction;
    target.Friction = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Restitution, ref target3, hookCtx, false, context))
      target3 = this.Restitution;
    target.Restitution = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hard, ref target4, hookCtx, false, context))
      target4 = this.Hard;
    target.Hard = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Density, ref target5, hookCtx, false, context))
      target5 = this.Density;
    target.Density = target5;
    int copy1 = serialization.CreateCopy<int, FlagSerializer<Robust.Shared.Physics.Dynamics.CollisionLayer>>(this.CollisionLayer, hookCtx, context);
    target.CollisionLayer = copy1;
    int copy2 = serialization.CreateCopy<int, FlagSerializer<Robust.Shared.Physics.Dynamics.CollisionMask>>(this.CollisionMask, hookCtx, context);
    target.CollisionMask = copy2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Fixture target,
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
    Fixture target1 = (Fixture) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Fixture Instantiate() => new Fixture();
}
