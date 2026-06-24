// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.Joint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Joints;

[ImplicitDataDefinitionForInheritors]
public abstract class Joint : 
  IEquatable<Joint>,
  ISerializationGenerated<Joint>,
  ISerializationGenerated
{
  internal bool IslandFlag;
  [DataField("localAnchorA", false, 1, false, false, null)]
  private Vector2 _localAnchorA;
  [DataField("localAnchorB", false, 1, false, false, null)]
  private Vector2 _localAnchorB;
  [DataField("collideConnected", false, 1, false, false, null)]
  protected bool _collideConnected = true;
  [DataField("breakpoint", false, 1, false, false, null)]
  private float _breakpoint = float.MaxValue;
  private double _breakpointSquared = double.MaxValue;

  [DataField("id", false, 1, false, false, null)]
  public string ID { get; set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled { get; internal set; } = true;

  public abstract JointType JointType { get; }

  [DataField("bodyA", false, 1, false, false, null)]
  public EntityUid BodyAUid { get; private set; }

  [DataField("bodyB", false, 1, false, false, null)]
  public EntityUid BodyBUid { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 LocalAnchorA
  {
    get => this._localAnchorA;
    set
    {
      if (Vector2Helpers.EqualsApprox(this._localAnchorA, value))
        return;
      this._localAnchorA = value;
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 LocalAnchorB
  {
    get => this._localAnchorB;
    set
    {
      if (Vector2Helpers.EqualsApprox(this._localAnchorB, value))
        return;
      this._localAnchorB = value;
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool CollideConnected
  {
    get => this._collideConnected;
    set
    {
      if (this._collideConnected == value)
        return;
      this._collideConnected = value;
      if (!this._collideConnected)
        IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedJointSystem>().FilterContactsForJoint(this);
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Breakpoint
  {
    get => this._breakpoint;
    set
    {
      if (MathHelper.CloseToPercent(this._breakpoint, value, 1E-05))
        return;
      this._breakpoint = value;
      this._breakpointSquared = (double) this._breakpoint * (double) this._breakpoint;
      this.Dirty();
    }
  }

  public EntityUid GetOther(EntityUid uid)
  {
    if (this.BodyAUid == uid)
      return this.BodyBUid;
    if (this.BodyBUid == uid)
      return this.BodyAUid;
    throw new ArgumentOutOfRangeException($"EntityUid {uid} unrelated to joint");
  }

  protected internal void Dirty(IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    PhysicsComponent component;
    if (entMan.TryGetComponent<PhysicsComponent>(this.BodyAUid, out component))
      entMan.Dirty(this.BodyAUid, (IComponent) component);
    if (!entMan.TryGetComponent<PhysicsComponent>(this.BodyBUid, out component))
      return;
    entMan.Dirty(this.BodyBUid, (IComponent) component);
  }

  protected Joint()
  {
  }

  protected Joint(EntityUid bodyAUid, EntityUid bodyBUid)
  {
    this.BodyAUid = bodyAUid;
    this.BodyBUid = bodyBUid;
  }

  protected Joint(JointState state, IEntityManager entManager, EntityUid owner)
  {
    this.ID = state.ID;
    this.BodyAUid = entManager.EnsureEntity<JointComponent>(state.UidA, owner);
    this.BodyBUid = entManager.EnsureEntity<JointComponent>(state.UidB, owner);
    this.Enabled = state.Enabled;
    this._collideConnected = state.CollideConnected;
    this._localAnchorA = state.LocalAnchorA;
    this._localAnchorB = state.LocalAnchorB;
    this._breakpoint = state.Breakpoint;
  }

  protected void GetState(JointState state, IEntityManager entManager)
  {
    state.ID = this.ID;
    state.CollideConnected = this._collideConnected;
    state.Enabled = this.Enabled;
    state.UidA = entManager.GetNetEntity(this.BodyAUid);
    state.UidB = entManager.GetNetEntity(this.BodyBUid);
    state.Breakpoint = this._breakpoint;
  }

  public abstract JointState GetState(IEntityManager entManager);

  internal virtual void ApplyState(JointState state)
  {
    this.ID = state.ID;
    this.CollideConnected = state.CollideConnected;
    this.Enabled = state.Enabled;
    this.Breakpoint = state.Breakpoint;
    this._breakpointSquared = (double) this.Breakpoint * (double) this.Breakpoint;
    this._localAnchorA = state.LocalAnchorA;
    this._localAnchorB = state.LocalAnchorB;
  }

  public abstract Vector2 GetReactionForce(float invDt);

  public abstract float GetReactionTorque(float invDt);

  internal abstract void InitVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    PhysicsComponent bodyA,
    PhysicsComponent bodyB,
    Vector2[] positions,
    float[] angles,
    Vector2[] linearVelocities,
    float[] angularVelocities);

  internal float Validate(float invDt)
  {
    if (!this.Enabled)
      return 0.0f;
    float x = this.GetReactionForce(invDt).LengthSquared();
    if ((double) MathF.Abs(x) <= this._breakpointSquared)
      return 0.0f;
    this.Enabled = false;
    return x;
  }

  internal abstract void SolveVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    Vector2[] linearVelocities,
    float[] angularVelocities);

  internal abstract bool SolvePositionConstraints(
    in SolverData data,
    Vector2[] positions,
    float[] angles);

  public bool Equals(Joint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    if (this.Enabled == other.Enabled && this.JointType == other.JointType)
    {
      EntityUid entityUid = this.BodyAUid;
      if (entityUid.Equals(other.BodyAUid))
      {
        entityUid = this.BodyBUid;
        if (entityUid.Equals(other.BodyBUid) && this.CollideConnected == other.CollideConnected && MathHelper.CloseTo(this._breakpoint, other._breakpoint, 1E-07f) && Vector2Helpers.EqualsApprox(this._localAnchorA, other._localAnchorA))
          return Vector2Helpers.EqualsApprox(this._localAnchorB, other._localAnchorB);
      }
    }
    return false;
  }

  public override int GetHashCode()
  {
    EntityUid entityUid = this.BodyAUid;
    int num = entityUid.GetHashCode() * 397;
    entityUid = this.BodyBUid;
    int hashCode = entityUid.GetHashCode();
    return (num ^ hashCode) * 397 ^ this.JointType.GetHashCode();
  }

  public abstract Joint Clone(EntityUid uidA, EntityUid uidB);

  public Joint Clone() => this.Clone(this.BodyAUid, this.BodyBUid);

  public abstract void CopyTo(Joint original);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref Joint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Joint>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    EntityUid target3 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.BodyAUid, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid>(this.BodyAUid, hookCtx, context);
    target.BodyAUid = target3;
    EntityUid target4 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.BodyBUid, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid>(this.BodyBUid, hookCtx, context);
    target.BodyBUid = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this._localAnchorA, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this._localAnchorA, hookCtx, context);
    target._localAnchorA = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this._localAnchorB, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this._localAnchorB, hookCtx, context);
    target._localAnchorB = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this._collideConnected, ref target7, hookCtx, false, context))
      target7 = this._collideConnected;
    target._collideConnected = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._breakpoint, ref target8, hookCtx, false, context))
      target8 = this._breakpoint;
    target._breakpoint = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Joint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual Joint Instantiate() => throw new NotImplementedException();
}
