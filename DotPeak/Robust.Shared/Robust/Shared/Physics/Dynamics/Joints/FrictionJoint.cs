// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.FrictionJoint
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

public sealed class FrictionJoint : 
  Joint,
  IEquatable<FrictionJoint>,
  ISerializationGenerated<FrictionJoint>,
  ISerializationGenerated
{
  private Vector2 _linearImpulse;
  private float _angularImpulse;
  private int _indexA;
  private int _indexB;
  private Vector2 _rA;
  private Vector2 _rB;
  private Vector2 _localCenterA;
  private Vector2 _localCenterB;
  private float _invMassA;
  private float _invMassB;
  private float _invIA;
  private float _invIB;
  private float _angularMass;
  private Vector2[] _linearMass = new Vector2[2];

  public override JointType JointType => JointType.Friction;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxForce", false, 1, false, false, null)]
  public float MaxForce { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxTorque", false, 1, false, false, null)]
  public float MaxTorque { get; set; }

  public FrictionJoint()
  {
  }

  public FrictionJoint(EntityUid uidA, EntityUid uidB, Vector2 anchorA, Vector2 anchorB)
    : base(uidA, uidB)
  {
    this.LocalAnchorA = anchorA;
    this.LocalAnchorB = anchorB;
  }

  internal FrictionJoint(FrictionJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this.MaxForce = state.MaxForce;
    this.MaxTorque = state.MaxTorque;
  }

  public override JointState GetState(IEntityManager entManager)
  {
    FrictionJointState state = new FrictionJointState();
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  internal override void ApplyState(JointState state)
  {
    base.ApplyState(state);
    if (!(state is FrictionJointState frictionJointState))
      return;
    this.MaxForce = frictionJointState.MaxForce;
    this.MaxTorque = frictionJointState.MaxTorque;
  }

  public override Vector2 GetReactionForce(float invDt) => this._linearImpulse * invDt;

  public override float GetReactionTorque(float invDt) => invDt * this._angularImpulse;

  internal override unsafe void InitVelocityConstraints(
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
    this._indexA = bodyA.IslandIndex[island.Index];
    this._indexB = bodyB.IslandIndex[island.Index];
    this._localCenterA = bodyA.LocalCenter;
    this._localCenterB = bodyB.LocalCenter;
    this._invMassA = bodyA.InvMass;
    this._invMassB = bodyB.InvMass;
    this._invIA = bodyA.InvI;
    this._invIB = bodyB.InvI;
    float angle1 = angles[this._indexA];
    Vector2 linearVelocity1 = linearVelocities[offset + this._indexA];
    float angularVelocity1 = angularVelocities[offset + this._indexA];
    float angle2 = angles[this._indexB];
    Vector2 linearVelocity2 = linearVelocities[offset + this._indexB];
    float angularVelocity2 = angularVelocities[offset + this._indexB];
    Quaternion2D quaternion2D1 = new Quaternion2D(angle1);
    Quaternion2D quaternion2D2 = new Quaternion2D(angle2);
    this._rA = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    this._rB = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    ; // Unable to render the statement
    Span<Vector2> matrix = new Span<Vector2>((void*) pointer, 2);
    matrix[0].X = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) this._rA.Y * (double) this._rA.Y + (double) invIb * (double) this._rB.Y * (double) this._rB.Y);
    matrix[0].Y = (float) (-(double) invIa * (double) this._rA.X * (double) this._rA.Y - (double) invIb * (double) this._rB.X * (double) this._rB.Y);
    matrix[1].X = matrix[0].Y;
    matrix[1].Y = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) this._rA.X * (double) this._rA.X + (double) invIb * (double) this._rB.X * (double) this._rB.X);
    Vector4Helpers.Inverse(matrix);
    this._angularMass = invIa + invIb;
    if ((double) this._angularMass > 0.0)
      this._angularMass = 1f / this._angularMass;
    if (data.WarmStarting)
    {
      this._linearImpulse *= data.DtRatio;
      this._angularImpulse *= data.DtRatio;
      Vector2 vector2 = new Vector2(this._linearImpulse.X, this._linearImpulse.Y);
      linearVelocity1 -= vector2 * invMassA;
      angularVelocity1 -= invIa * (Vector2Helpers.Cross(this._rA, vector2) + this._angularImpulse);
      linearVelocity2 += vector2 * invMassB;
      angularVelocity2 += invIb * (Vector2Helpers.Cross(this._rB, vector2) + this._angularImpulse);
    }
    else
    {
      this._linearImpulse = Vector2.Zero;
      this._angularImpulse = 0.0f;
    }
    linearVelocities[offset + this._indexA] = linearVelocity1;
    angularVelocities[offset + this._indexA] = angularVelocity1;
    linearVelocities[offset + this._indexB] = linearVelocity2;
    angularVelocities[offset + this._indexB] = angularVelocity2;
  }

  internal override void SolveVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int offset = island.Offset;
    Vector2 linearVelocity1 = linearVelocities[offset + this._indexA];
    float angularVelocity1 = angularVelocities[offset + this._indexA];
    Vector2 linearVelocity2 = linearVelocities[offset + this._indexB];
    float angularVelocity2 = angularVelocities[offset + this._indexB];
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    double frameTime = (double) data.FrameTime;
    float num1 = -this._angularMass * (angularVelocity2 - angularVelocity1);
    float angularImpulse = this._angularImpulse;
    float max = (float) frameTime * this.MaxTorque;
    this._angularImpulse = Math.Clamp(this._angularImpulse + num1, -max, max);
    float num2 = this._angularImpulse - angularImpulse;
    float num3 = angularVelocity1 - invIa * num2;
    float num4 = angularVelocity2 + invIb * num2;
    Vector2 v = linearVelocity2 + Vector2Helpers.Cross(num4, ref this._rB) - linearVelocity1 - Vector2Helpers.Cross(num3, ref this._rA);
    Vector2 vector2_1 = -Transform.Mul(in this._linearMass, in v);
    Vector2 linearImpulse = this._linearImpulse;
    this._linearImpulse += vector2_1;
    float num5 = (float) frameTime * this.MaxForce;
    if ((double) this._linearImpulse.LengthSquared() > (double) num5 * (double) num5)
    {
      this._linearImpulse = Vector2Helpers.Normalized(this._linearImpulse);
      this._linearImpulse *= num5;
    }
    Vector2 vector2_2 = this._linearImpulse - linearImpulse;
    Vector2 vector2_3 = linearVelocity1 - vector2_2 * invMassA;
    float num6 = num3 - invIa * Vector2Helpers.Cross(this._rA, vector2_2);
    Vector2 vector2_4 = linearVelocity2 + vector2_2 * invMassB;
    float num7 = num4 + invIb * Vector2Helpers.Cross(this._rB, vector2_2);
    linearVelocities[offset + this._indexA] = vector2_3;
    angularVelocities[offset + this._indexA] = num6;
    linearVelocities[offset + this._indexB] = vector2_4;
    angularVelocities[offset + this._indexB] = num7;
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
    FrictionJoint frictionJoint = new FrictionJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB);
    frictionJoint.Enabled = this.Enabled;
    frictionJoint.MaxTorque = this.MaxTorque;
    frictionJoint.MaxForce = this.MaxForce;
    frictionJoint._linearImpulse = this._linearImpulse;
    frictionJoint._angularImpulse = this._angularImpulse;
    frictionJoint.Breakpoint = this.Breakpoint;
    return (Joint) frictionJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is FrictionJoint frictionJoint))
      return;
    frictionJoint.Enabled = this.Enabled;
    frictionJoint.MaxTorque = this.MaxTorque;
    frictionJoint.MaxForce = this.MaxForce;
    frictionJoint._linearImpulse = this._linearImpulse;
    frictionJoint._angularImpulse = this._angularImpulse;
    frictionJoint.Breakpoint = this.Breakpoint;
  }

  public bool Equals(FrictionJoint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.Equals((Joint) other) && MathHelper.CloseTo(this.MaxForce, other.MaxForce, 1E-07f) && MathHelper.CloseTo(this.MaxTorque, other.MaxTorque, 1E-07f);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FrictionJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FrictionJoint) target1;
    if (serialization.TryCustomCopy<FrictionJoint>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxForce, ref target2, hookCtx, false, context))
      target2 = this.MaxForce;
    target.MaxForce = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTorque, ref target3, hookCtx, false, context))
      target3 = this.MaxTorque;
    target.MaxTorque = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FrictionJoint target,
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
    FrictionJoint target1 = (FrictionJoint) target;
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
    FrictionJoint target1 = (FrictionJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FrictionJoint Joint.Instantiate() => new FrictionJoint();
}
