// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.MouseJoint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class MouseJoint : 
  Joint,
  IEquatable<MouseJoint>,
  ISerializationGenerated<MouseJoint>,
  ISerializationGenerated
{
  private float _maxForce;
  private float _stiffness;
  private float _damping;
  private float _invMassB;
  private float _invIB;
  private Vector2 _rB;
  private Vector2 _C;
  private Matrix22 _mass;
  private Vector2 _impulse;
  private float _beta;
  private float _gamma;
  private int _indexB;
  private Vector2 _localCenterB;

  public override JointType JointType => JointType.Mouse;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxForce", false, 1, false, false, null)]
  public float MaxForce
  {
    get => this._maxForce;
    set
    {
      if (MathHelper.CloseTo(this._maxForce, value, 1E-07f))
        return;
      this._maxForce = value;
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("stiffness", false, 1, false, false, null)]
  public float Stiffness
  {
    get => this._stiffness;
    set
    {
      if (MathHelper.CloseTo(this._stiffness, value, 1E-07f))
        return;
      this._stiffness = value;
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("damping", false, 1, false, false, null)]
  public float Damping
  {
    get => this._damping;
    set
    {
      if (MathHelper.CloseTo(this._damping, value, 1E-07f))
        return;
      this._damping = value;
      this.Dirty();
    }
  }

  public MouseJoint()
  {
  }

  public MouseJoint(EntityUid uidA, EntityUid uidB, Vector2 localAnchorA, Vector2 localAnchorB)
    : base(uidA, uidB)
  {
    this.LocalAnchorA = localAnchorA;
    this.LocalAnchorB = localAnchorB;
  }

  internal MouseJoint(MouseJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this.Damping = state.Damping;
    this.Stiffness = state.Stiffness;
    this.MaxForce = state.MaxForce;
  }

  public override JointState GetState(IEntityManager entManager)
  {
    MouseJointState mouseJointState = new MouseJointState();
    mouseJointState.Damping = this._damping;
    mouseJointState.Stiffness = this._stiffness;
    mouseJointState.MaxForce = this._maxForce;
    mouseJointState.LocalAnchorA = this.LocalAnchorA;
    mouseJointState.LocalAnchorB = this.LocalAnchorB;
    MouseJointState state = mouseJointState;
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  public override Vector2 GetReactionForce(float invDt) => this._impulse * invDt;

  public override float GetReactionTorque(float invDt) => invDt * 0.0f;

  internal override void InitVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    PhysicsComponent bodyA,
    PhysicsComponent bodyB,
    Vector2[] positions,
    float[] angles,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int offset = island.Offset;
    this._indexB = bodyB.IslandIndex[island.Index];
    this._localCenterB = bodyB.LocalCenter;
    this._invMassB = bodyB.InvMass;
    this._invIB = bodyB.InvI;
    Vector2 position1 = positions[this._indexB];
    float angle = angles[this._indexB];
    Vector2 linearVelocity = linearVelocities[offset + this._indexB];
    float angularVelocity = angularVelocities[offset + this._indexB];
    Quaternion2D quaternion2D = new Quaternion2D(angle);
    float damping = this._damping;
    float stiffness = this._stiffness;
    float frameTime = data.FrameTime;
    this._gamma = frameTime * (damping + frameTime * stiffness);
    if ((double) this._gamma != 0.0)
      this._gamma = 1f / this._gamma;
    this._beta = frameTime * stiffness * this._gamma;
    this._rB = Transform.Mul(in quaternion2D, this.LocalAnchorB - this._localCenterB);
    Matrix22 matrix22;
    matrix22.EX.X = this._invMassB + this._invIB * this._rB.Y * this._rB.Y + this._gamma;
    matrix22.EX.Y = -this._invIB * this._rB.X * this._rB.Y;
    matrix22.EY.X = matrix22.EX.Y;
    matrix22.EY.Y = this._invMassB + this._invIB * this._rB.X * this._rB.X + this._gamma;
    this._mass = ((Matrix22) ref matrix22).GetInverse();
    Vector2 position2 = positions[bodyA.IslandIndex[island.Index]];
    this._C = position1 + this._rB - position2;
    this._C *= this._beta;
    float num = angularVelocity * MathF.Max(0.0f, (float) (1.0 - 0.019999999552965164 * (60.0 * (double) data.FrameTime)));
    if (data.WarmStarting)
    {
      this._impulse *= data.DtRatio;
      linearVelocity += this._impulse * this._invMassB;
      num += this._invIB * Vector2Helpers.Cross(this._rB, this._impulse);
    }
    else
      this._impulse = Vector2.Zero;
    linearVelocities[offset + this._indexB] = linearVelocity;
    angularVelocities[offset + this._indexB] = num;
  }

  internal override void SolveVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int offset = island.Offset;
    Vector2 linearVelocity = linearVelocities[offset + this._indexB];
    float angularVelocity = angularVelocities[offset + this._indexB];
    Vector2 vector2_1 = Transform.Mul(this._mass, -(linearVelocity + Vector2Helpers.Cross(angularVelocity, ref this._rB) + this._C + this._impulse * this._gamma));
    Vector2 impulse = this._impulse;
    this._impulse += vector2_1;
    float num1 = data.FrameTime * this._maxForce;
    if ((double) this._impulse.LengthSquared() > (double) num1 * (double) num1)
      this._impulse *= num1 / this._impulse.Length();
    Vector2 vector2_2 = this._impulse - impulse;
    Vector2 vector2_3 = linearVelocity + vector2_2 * this._invMassB;
    float num2 = angularVelocity + this._invIB * Vector2Helpers.Cross(this._rB, vector2_2);
    linearVelocities[offset + this._indexB] = vector2_3;
    angularVelocities[offset + this._indexB] = num2;
  }

  internal override bool SolvePositionConstraints(
    in SolverData data,
    Vector2[] positions,
    float[] angles)
  {
    return true;
  }

  public override Joint Clone(EntityUid uidA, EntityUid uidB)
  {
    MouseJoint mouseJoint = new MouseJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB);
    mouseJoint.Enabled = this.Enabled;
    mouseJoint.MaxForce = this.MaxForce;
    mouseJoint.Damping = this.Damping;
    mouseJoint.Stiffness = this.Stiffness;
    mouseJoint._impulse = this._impulse;
    mouseJoint.Breakpoint = this.Breakpoint;
    return (Joint) mouseJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is MouseJoint mouseJoint))
      return;
    mouseJoint.Enabled = this.Enabled;
    mouseJoint.MaxForce = this.MaxForce;
    mouseJoint.Damping = this.Damping;
    mouseJoint.Stiffness = this.Stiffness;
    mouseJoint._impulse = this._impulse;
    mouseJoint.Breakpoint = this.Breakpoint;
  }

  public bool Equals(MouseJoint? other)
  {
    return other != null && this.BodyAUid == other.BodyAUid && this.BodyBUid == other.BodyBUid && this._damping.Equals(other.Damping) && this._stiffness.Equals(other.Stiffness) && this._maxForce.Equals(other.MaxForce);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MouseJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MouseJoint) target1;
    if (serialization.TryCustomCopy<MouseJoint>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxForce, ref target2, hookCtx, false, context))
      target2 = this.MaxForce;
    target.MaxForce = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Stiffness, ref target3, hookCtx, false, context))
      target3 = this.Stiffness;
    target.Stiffness = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Damping, ref target4, hookCtx, false, context))
      target4 = this.Damping;
    target.Damping = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MouseJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Joint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MouseJoint target1 = (MouseJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Joint) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MouseJoint target1 = (MouseJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MouseJoint Joint.Instantiate() => new MouseJoint();
}
