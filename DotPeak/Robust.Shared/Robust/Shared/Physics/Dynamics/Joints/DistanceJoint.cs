// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.DistanceJoint
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

public sealed class DistanceJoint : 
  Joint,
  IEquatable<DistanceJoint>,
  ISerializationGenerated<DistanceJoint>,
  ISerializationGenerated
{
  private float _bias;
  private float _gamma;
  private float _impulse;
  private float _lowerImpulse;
  private float _upperImpulse;
  private int _indexA;
  private int _indexB;
  private Vector2 _u;
  private Vector2 _rA;
  private Vector2 _rB;
  private Vector2 _localCenterA;
  private Vector2 _localCenterB;
  private float _invMassA;
  private float _invMassB;
  private float _invIA;
  private float _invIB;
  private float _mass;
  private float _currentLength;
  private float _softMass;
  private float _length;
  private float _maxLength;
  private float _minLength;
  private float _stiffness;
  private float _damping;

  public override JointType JointType => JointType.Distance;

  public DistanceJoint()
  {
  }

  public DistanceJoint(
    EntityUid uidA,
    EntityUid uidB,
    Vector2 anchorA,
    Vector2 anchorB,
    float length)
    : base(uidA, uidB)
  {
    this.Length = MathF.Max(0.005f, length);
    this._minLength = this._length;
    this._maxLength = this._length;
    this.LocalAnchorA = anchorA;
    this.LocalAnchorB = anchorB;
  }

  internal DistanceJoint(DistanceJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this._damping = state.Damping;
    this._length = state.Length;
    this._maxLength = state.MaxLength;
    this._minLength = state.MinLength;
    this._stiffness = state.Stiffness;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("length", false, 1, false, false, null)]
  public float Length
  {
    get => this._length;
    set
    {
      if (MathHelper.CloseTo(value, this._length, 1E-07f))
        return;
      this._impulse = 0.0f;
      this._length = MathF.Max(value, 0.005f);
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxLength", false, 1, false, false, null)]
  public float MaxLength
  {
    get => this._maxLength;
    set
    {
      if (MathHelper.CloseTo(value, this._maxLength, 1E-07f))
        return;
      this._upperImpulse = 0.0f;
      this._maxLength = MathF.Max(value, this._minLength);
      this.Dirty();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("minLength", false, 1, false, false, null)]
  public float MinLength
  {
    get => this._minLength;
    set
    {
      if (MathHelper.CloseTo(value, this._minLength, 1E-07f))
        return;
      this._lowerImpulse = 0.0f;
      this._minLength = Math.Clamp(value, 0.005f, this.MaxLength);
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

  public override Vector2 GetReactionForce(float invDt)
  {
    return this._u * invDt * (this._impulse + this._lowerImpulse - this._upperImpulse);
  }

  public override JointState GetState(IEntityManager entManager)
  {
    DistanceJointState distanceJointState = new DistanceJointState();
    distanceJointState.Damping = this._damping;
    distanceJointState.Length = this._length;
    distanceJointState.MinLength = this._minLength;
    distanceJointState.MaxLength = this._maxLength;
    distanceJointState.Stiffness = this._stiffness;
    distanceJointState.LocalAnchorA = this.LocalAnchorA;
    distanceJointState.LocalAnchorB = this.LocalAnchorB;
    DistanceJointState state = distanceJointState;
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  internal override void ApplyState(JointState state)
  {
    base.ApplyState(state);
    if (!(state is DistanceJointState distanceJointState))
      return;
    this._damping = distanceJointState.Damping;
    this._length = distanceJointState.Length;
    this._minLength = distanceJointState.MinLength;
    this._maxLength = distanceJointState.MaxLength;
    this._stiffness = distanceJointState.Stiffness;
  }

  public override float GetReactionTorque(float invDt) => 0.0f;

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
    this._indexA = bodyA.IslandIndex[island.Index];
    this._indexB = bodyB.IslandIndex[island.Index];
    this._localCenterA = bodyA.LocalCenter;
    this._localCenterB = bodyB.LocalCenter;
    this._invMassA = bodyA.InvMass;
    this._invMassB = bodyB.InvMass;
    this._invIA = bodyA.InvI;
    this._invIB = bodyB.InvI;
    Vector2 position1 = positions[this._indexA];
    float angle1 = angles[this._indexA];
    Vector2 linearVelocity1 = linearVelocities[offset + this._indexA];
    float angularVelocity1 = angularVelocities[offset + this._indexA];
    Vector2 position2 = positions[this._indexB];
    float angle2 = angles[this._indexB];
    Vector2 linearVelocity2 = linearVelocities[offset + this._indexB];
    float angularVelocity2 = angularVelocities[offset + this._indexB];
    Quaternion2D quaternion2D1 = new Quaternion2D(angle1);
    Quaternion2D quaternion2D2 = new Quaternion2D(angle2);
    this._rA = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    this._rB = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    this._u = position2 + this._rB - position1 - this._rA;
    this._currentLength = this._u.Length();
    if ((double) this._currentLength > 0.004999999888241291)
    {
      this._u *= 1f / this._currentLength;
    }
    else
    {
      this._u = Vector2.Zero;
      this._mass = 0.0f;
      this._impulse = 0.0f;
      this._lowerImpulse = 0.0f;
      this._upperImpulse = 0.0f;
    }
    float num1 = Vector2Helpers.Cross(this._rA, this._u);
    float num2 = Vector2Helpers.Cross(this._rB, this._u);
    float num3 = (float) ((double) this._invMassA + (double) this._invIA * (double) num1 * (double) num1 + (double) this._invMassB + (double) this._invIB * (double) num2 * (double) num2);
    this._mass = (double) num3 != 0.0 ? 1f / num3 : 0.0f;
    if ((double) this.Stiffness > 0.0 && (double) this._minLength < (double) this._maxLength)
    {
      float num4 = this._currentLength - this._length;
      float damping = this.Damping;
      float stiffness = this.Stiffness;
      float frameTime = data.FrameTime;
      this._gamma = frameTime * (damping + frameTime * stiffness);
      this._gamma = (double) this._gamma != 0.0 ? 1f / this._gamma : 0.0f;
      this._bias = num4 * frameTime * stiffness * this._gamma;
      float num5 = num3 + this._gamma;
      this._softMass = (double) num5 != 0.0 ? 1f / num5 : 0.0f;
    }
    else
    {
      this._gamma = 0.0f;
      this._bias = 0.0f;
      this._softMass = this._mass;
    }
    if (data.WarmStarting)
    {
      this._impulse *= data.DtRatio;
      this._lowerImpulse *= data.DtRatio;
      this._upperImpulse *= data.DtRatio;
      Vector2 vector2 = this._u * (this._impulse + this._lowerImpulse - this._upperImpulse);
      linearVelocity1 -= vector2 * this._invMassA;
      angularVelocity1 -= this._invIA * Vector2Helpers.Cross(this._rA, vector2);
      linearVelocity2 += vector2 * this._invMassB;
      angularVelocity2 += this._invIB * Vector2Helpers.Cross(this._rB, vector2);
    }
    else
      this._impulse = 0.0f;
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
    Vector2 vector2_1;
    float num1;
    Vector2 vector2_2;
    float num2;
    if ((double) this._minLength < (double) this._maxLength)
    {
      if ((double) this.Stiffness > 0.0)
      {
        Vector2 vector2_3 = linearVelocity1 + Vector2Helpers.Cross(angularVelocity1, ref this._rA);
        float num3 = (float) (-(double) this._softMass * ((double) Vector2.Dot(this._u, linearVelocity2 + Vector2Helpers.Cross(angularVelocity2, ref this._rB) - vector2_3) + (double) this._bias + (double) this._gamma * (double) this._impulse));
        this._impulse += num3;
        Vector2 vector2_4 = this._u * num3;
        linearVelocity1 -= vector2_4 * this._invMassA;
        angularVelocity1 -= this._invIA * Vector2Helpers.Cross(this._rA, vector2_4);
        linearVelocity2 += vector2_4 * this._invMassB;
        angularVelocity2 += this._invIB * Vector2Helpers.Cross(this._rB, vector2_4);
      }
      float num4 = MathF.Max(0.0f, this._currentLength - this._minLength) * data.InvDt;
      Vector2 vector2_5 = linearVelocity1 + Vector2Helpers.Cross(angularVelocity1, ref this._rA);
      float num5 = (float) (-(double) this._mass * ((double) Vector2.Dot(this._u, linearVelocity2 + Vector2Helpers.Cross(angularVelocity2, ref this._rB) - vector2_5) + (double) num4));
      float lowerImpulse = this._lowerImpulse;
      this._lowerImpulse = MathF.Max(0.0f, this._lowerImpulse + num5);
      Vector2 vector2_6 = this._u * (this._lowerImpulse - lowerImpulse);
      Vector2 vector2_7 = linearVelocity1 - vector2_6 * this._invMassA;
      float num6 = angularVelocity1 - this._invIA * Vector2Helpers.Cross(this._rA, vector2_6);
      Vector2 vector2_8 = linearVelocity2 + vector2_6 * this._invMassB;
      float num7 = angularVelocity2 + this._invIB * Vector2Helpers.Cross(this._rB, vector2_6);
      float num8 = MathF.Max(0.0f, this._maxLength - this._currentLength) * data.InvDt;
      float num9 = (float) (-(double) this._mass * ((double) Vector2.Dot(this._u, vector2_7 + Vector2Helpers.Cross(num6, ref this._rA) - (vector2_8 + Vector2Helpers.Cross(num7, ref this._rB))) + (double) num8));
      float upperImpulse = this._upperImpulse;
      this._upperImpulse = MathF.Max(0.0f, this._upperImpulse + num9);
      Vector2 vector2_9 = this._u * -(this._upperImpulse - upperImpulse);
      vector2_1 = vector2_7 - vector2_9 * this._invMassA;
      num1 = num6 - this._invIA * Vector2Helpers.Cross(this._rA, vector2_9);
      vector2_2 = vector2_8 + vector2_9 * this._invMassB;
      num2 = num7 + this._invIB * Vector2Helpers.Cross(this._rB, vector2_9);
    }
    else
    {
      Vector2 vector2_10 = linearVelocity1 + Vector2Helpers.Cross(angularVelocity1, ref this._rA);
      float num10 = -this._mass * Vector2.Dot(this._u, linearVelocity2 + Vector2Helpers.Cross(angularVelocity2, ref this._rB) - vector2_10);
      this._impulse += num10;
      Vector2 vector2_11 = this._u * num10;
      vector2_1 = linearVelocity1 - vector2_11 * this._invMassA;
      num1 = angularVelocity1 - this._invIA * Vector2Helpers.Cross(this._rA, vector2_11);
      vector2_2 = linearVelocity2 + vector2_11 * this._invMassB;
      num2 = angularVelocity2 + this._invIB * Vector2Helpers.Cross(this._rB, vector2_11);
    }
    linearVelocities[offset + this._indexA] = vector2_1;
    angularVelocities[offset + this._indexA] = num1;
    linearVelocities[offset + this._indexB] = vector2_2;
    angularVelocities[offset + this._indexB] = num2;
  }

  internal override bool SolvePositionConstraints(
    in SolverData data,
    Vector2[] positions,
    float[] angles)
  {
    Vector2 position1 = positions[this._indexA];
    float angle1 = angles[this._indexA];
    Vector2 position2 = positions[this._indexB];
    float angle2 = angles[this._indexB];
    Quaternion2D quaternion2D1 = new Quaternion2D(angle1);
    Quaternion2D quaternion2D2 = new Quaternion2D(angle2);
    Vector2 vector2_1 = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    Vector2 vector2_2 = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    Vector2 vector2_3 = position2 + vector2_2 - position1 - vector2_1;
    float num1 = vector2_3.Length();
    Vector2 vector2_4 = Vector2Helpers.Normalized(vector2_3);
    float x;
    if (MathHelper.CloseTo(this._minLength, this._maxLength, 1E-07f))
      x = num1 - this._minLength;
    else if ((double) num1 < (double) this._minLength)
    {
      x = num1 - this._minLength;
    }
    else
    {
      if ((double) this._maxLength >= (double) num1)
        return true;
      x = num1 - this._maxLength;
    }
    float num2 = -this._mass * x;
    Vector2 vector2_5 = vector2_4 * num2;
    Vector2 vector2_6 = position1 - vector2_5 * this._invMassA;
    float num3 = angle1 - this._invIA * Vector2Helpers.Cross(vector2_1, vector2_5);
    Vector2 vector2_7 = position2 + vector2_5 * this._invMassB;
    float num4 = angle2 + this._invIB * Vector2Helpers.Cross(vector2_2, vector2_5);
    positions[this._indexA] = vector2_6;
    angles[this._indexA] = num3;
    positions[this._indexB] = vector2_7;
    angles[this._indexB] = num4;
    return (double) MathF.Abs(x) < 0.004999999888241291;
  }

  public override Joint Clone(EntityUid uidA, EntityUid uidB)
  {
    DistanceJoint distanceJoint = new DistanceJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB, this.Length);
    distanceJoint.Enabled = this.Enabled;
    distanceJoint.MinLength = this.MinLength;
    distanceJoint.MaxLength = this.MaxLength;
    distanceJoint.Stiffness = this.Stiffness;
    distanceJoint.Damping = this.Damping;
    distanceJoint._lowerImpulse = this._lowerImpulse;
    distanceJoint._upperImpulse = this._upperImpulse;
    distanceJoint._impulse = this._impulse;
    distanceJoint.Breakpoint = this.Breakpoint;
    return (Joint) distanceJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is DistanceJoint distanceJoint))
      return;
    distanceJoint.Enabled = this.Enabled;
    distanceJoint.MinLength = this.MinLength;
    distanceJoint.MaxLength = this.MaxLength;
    distanceJoint.Length = this.Length;
    distanceJoint.Stiffness = this.Stiffness;
    distanceJoint.Damping = this.Damping;
    distanceJoint._lowerImpulse = this._lowerImpulse;
    distanceJoint._upperImpulse = this._upperImpulse;
    distanceJoint._impulse = this._impulse;
    distanceJoint.Breakpoint = this.Breakpoint;
  }

  public bool Equals(DistanceJoint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.Equals((Joint) other) && MathHelper.CloseTo(this._length, other._length, 1E-07f) && MathHelper.CloseTo(this._minLength, other._minLength, 1E-07f) && MathHelper.CloseTo(this._maxLength, other._maxLength, 1E-07f) && MathHelper.CloseTo(this._stiffness, other._stiffness, 1E-07f) && MathHelper.CloseTo(this._damping, other._damping, 1E-07f);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DistanceJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DistanceJoint) target1;
    if (serialization.TryCustomCopy<DistanceJoint>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Length, ref target2, hookCtx, false, context))
      target2 = this.Length;
    target.Length = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLength, ref target3, hookCtx, false, context))
      target3 = this.MaxLength;
    target.MaxLength = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinLength, ref target4, hookCtx, false, context))
      target4 = this.MinLength;
    target.MinLength = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Stiffness, ref target5, hookCtx, false, context))
      target5 = this.Stiffness;
    target.Stiffness = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Damping, ref target6, hookCtx, false, context))
      target6 = this.Damping;
    target.Damping = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DistanceJoint target,
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
    DistanceJoint target1 = (DistanceJoint) target;
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
    DistanceJoint target1 = (DistanceJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DistanceJoint Joint.Instantiate() => new DistanceJoint();
}
