// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.RevoluteJoint
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
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class RevoluteJoint : 
  Joint,
  IEquatable<RevoluteJoint>,
  ISerializationGenerated<RevoluteJoint>,
  ISerializationGenerated
{
  private Vector2 _impulse;
  private int _indexA;
  private int _indexB;
  private Vector2 _localCenterA;
  private Vector2 _localCenterB;
  private float _invMassA;
  private float _invMassB;
  private float _invIA;
  private float _invIB;
  private Vector2 _rA;
  private Vector2 _rB;
  private Matrix22 _K;
  private float _axialMass;
  private float _angle;
  private float _motorImpulse;
  private float _lowerImpulse;
  private float _upperImpulse;
  [DataField("enableLimit", false, 1, false, false, null)]
  public bool EnableLimit;
  [DataField("enableMotor", false, 1, false, false, null)]
  public bool EnableMotor;
  [DataField("referenceAngle", false, 1, false, false, null)]
  public float ReferenceAngle;
  [DataField("lowerAngle", false, 1, false, false, null)]
  public float LowerAngle;
  [DataField("upperAngle", false, 1, false, false, null)]
  public float UpperAngle;
  [DataField("motorSpeed", false, 1, false, false, null)]
  public float MotorSpeed;
  [DataField("maxMotorTorque", false, 1, false, false, null)]
  public float MaxMotorTorque;

  public RevoluteJoint()
  {
  }

  public RevoluteJoint(
    EntityUid uidA,
    EntityUid uidB,
    Vector2 anchorA,
    Vector2 anchorB,
    float referenceAngle)
    : base(uidA, uidB)
  {
    this.LocalAnchorA = anchorA;
    this.LocalAnchorB = anchorB;
    this.ReferenceAngle = referenceAngle;
  }

  public RevoluteJoint(EntityUid bodyAUid, EntityUid bodyBUid)
    : base(bodyAUid, bodyBUid)
  {
  }

  internal RevoluteJoint(RevoluteJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this.EnableLimit = state.EnableLimit;
    this.EnableMotor = state.EnableMotor;
    this.ReferenceAngle = state.ReferenceAngle;
    this.LowerAngle = state.LowerAngle;
    this.UpperAngle = state.UpperAngle;
    this.MotorSpeed = state.MotorSpeed;
    this.MaxMotorTorque = state.MaxMotorTorque;
  }

  public override JointType JointType => JointType.Revolute;

  public override JointState GetState(IEntityManager entManager)
  {
    RevoluteJointState state = new RevoluteJointState();
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  internal override void ApplyState(JointState state)
  {
    base.ApplyState(state);
    if (!(state is RevoluteJointState revoluteJointState))
      return;
    this.EnableLimit = revoluteJointState.EnableLimit;
    this.EnableMotor = revoluteJointState.EnableMotor;
    this.LowerAngle = revoluteJointState.LowerAngle;
    this.MotorSpeed = revoluteJointState.MotorSpeed;
    this.ReferenceAngle = revoluteJointState.ReferenceAngle;
    this.UpperAngle = revoluteJointState.UpperAngle;
    this.MaxMotorTorque = revoluteJointState.MaxMotorTorque;
  }

  public override Vector2 GetReactionForce(float invDt)
  {
    return new Vector2(this._impulse.X, this._impulse.Y) * invDt;
  }

  public override float GetReactionTorque(float invDt)
  {
    return invDt * (this._motorImpulse + this._lowerImpulse - this._upperImpulse);
  }

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
    this._K.EX.X = (float) ((double) invMassA + (double) invMassB + (double) this._rA.Y * (double) this._rA.Y * (double) invIa + (double) this._rB.Y * (double) this._rB.Y * (double) invIb);
    this._K.EY.X = (float) (-(double) this._rA.Y * (double) this._rA.X * (double) invIa - (double) this._rB.Y * (double) this._rB.X * (double) invIb);
    this._K.EX.Y = this._K.EY.X;
    this._K.EY.Y = (float) ((double) invMassA + (double) invMassB + (double) this._rA.X * (double) this._rA.X * (double) invIa + (double) this._rB.X * (double) this._rB.X * (double) invIb);
    this._axialMass = invIa + invIb;
    bool flag;
    if ((double) this._axialMass > 0.0)
    {
      this._axialMass = 1f / this._axialMass;
      flag = false;
    }
    else
      flag = true;
    this._angle = angle2 - angle1 - this.ReferenceAngle;
    if (!this.EnableLimit | flag)
    {
      this._lowerImpulse = 0.0f;
      this._upperImpulse = 0.0f;
    }
    if (!this.EnableMotor | flag)
      this._motorImpulse = 0.0f;
    if (data.WarmStarting)
    {
      this._impulse *= data.DtRatio;
      this._motorImpulse *= data.DtRatio;
      this._lowerImpulse *= data.DtRatio;
      this._upperImpulse *= data.DtRatio;
      float num = this._motorImpulse + this._lowerImpulse - this._upperImpulse;
      Vector2 vector2 = new Vector2(this._impulse.X, this._impulse.Y);
      linearVelocity1 -= vector2 * invMassA;
      angularVelocity1 -= invIa * (Vector2Helpers.Cross(this._rA, vector2) + num);
      linearVelocity2 += vector2 * invMassB;
      angularVelocity2 += invIb * (Vector2Helpers.Cross(this._rB, vector2) + num);
    }
    else
    {
      this._impulse = Vector2.Zero;
      this._motorImpulse = 0.0f;
      this._lowerImpulse = 0.0f;
      this._upperImpulse = 0.0f;
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
    float num1 = angularVelocities[offset + this._indexA];
    Vector2 linearVelocity2 = linearVelocities[offset + this._indexB];
    float num2 = angularVelocities[offset + this._indexB];
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    bool flag = (double) invIa + (double) invIb == 0.0;
    if (this.EnableMotor && !flag)
    {
      float num3 = -this._axialMass * (num2 - num1 - this.MotorSpeed);
      float motorImpulse = this._motorImpulse;
      float max = data.FrameTime * this.MaxMotorTorque;
      this._motorImpulse = Math.Clamp(this._motorImpulse + num3, -max, max);
      float num4 = this._motorImpulse - motorImpulse;
      num1 -= invIa * num4;
      num2 += invIb * num4;
    }
    if (this.EnableLimit && !flag)
    {
      float x1 = this._angle - this.LowerAngle;
      float num5 = (float) (-(double) this._axialMass * ((double) (num2 - num1) + (double) MathF.Max(x1, 0.0f) * (double) data.InvDt));
      float lowerImpulse = this._lowerImpulse;
      this._lowerImpulse = MathF.Max(this._lowerImpulse + num5, 0.0f);
      float num6 = this._lowerImpulse - lowerImpulse;
      float num7 = num1 - invIa * num6;
      float num8 = num2 + invIb * num6;
      float x2 = this.UpperAngle - this._angle;
      float num9 = (float) (-(double) this._axialMass * ((double) (num7 - num8) + (double) MathF.Max(x2, 0.0f) * (double) data.InvDt));
      float upperImpulse = this._upperImpulse;
      this._upperImpulse = MathF.Max(this._upperImpulse + num9, 0.0f);
      float num10 = this._upperImpulse - upperImpulse;
      num1 = num7 + invIa * num10;
      num2 = num8 - invIb * num10;
    }
    Vector2 vector2_1 = ((Matrix22) ref this._K).Solve(-(linearVelocity2 + Vector2Helpers.Cross(num2, ref this._rB) - linearVelocity1 - Vector2Helpers.Cross(num1, ref this._rA)));
    this._impulse.X += vector2_1.X;
    this._impulse.Y += vector2_1.Y;
    Vector2 vector2_2 = linearVelocity1 - vector2_1 * invMassA;
    float num11 = num1 - invIa * Vector2Helpers.Cross(this._rA, vector2_1);
    Vector2 vector2_3 = linearVelocity2 + vector2_1 * invMassB;
    float num12 = num2 + invIb * Vector2Helpers.Cross(this._rB, vector2_1);
    linearVelocities[offset + this._indexA] = vector2_2;
    angularVelocities[offset + this._indexA] = num11;
    linearVelocities[offset + this._indexB] = vector2_3;
    angularVelocities[offset + this._indexB] = num12;
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
    float num1 = 0.0f;
    bool flag = (double) this._invIA + (double) this._invIB == 0.0;
    if (this.EnableLimit && !flag)
    {
      float num2 = angle2 - angle1 - this.ReferenceAngle;
      float num3 = 0.0f;
      if ((double) Math.Abs(this.UpperAngle - this.LowerAngle) < 0.069813176989555359)
        num3 = Math.Clamp(num2 - this.LowerAngle, -data.MaxAngularCorrection, data.MaxAngularCorrection);
      else if ((double) num2 <= (double) this.LowerAngle)
        num3 = Math.Clamp((float) ((double) num2 - (double) this.LowerAngle + 0.034906588494777679), -data.MaxAngularCorrection, 0.0f);
      else if ((double) num2 >= (double) this.UpperAngle)
        num3 = Math.Clamp((float) ((double) num2 - (double) this.UpperAngle - 0.034906588494777679), 0.0f, data.MaxAngularCorrection);
      float num4 = -this._axialMass * num3;
      angle1 -= this._invIA * num4;
      angle2 += this._invIB * num4;
      num1 = Math.Abs(num3);
    }
    quaternion2D1.Set(angle1);
    quaternion2D2.Set(angle2);
    Vector2 vector2_1 = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    Vector2 vector2_2 = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    Vector2 vector2_3 = position2 + vector2_2 - position1 - vector2_1;
    double num5 = (double) vector2_3.Length();
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    Matrix22 matrix22;
    // ISSUE: explicit constructor call
    ((Matrix22) ref matrix22).\u002Ector((float) ((double) invMassA + (double) invMassB + (double) invIa * (double) vector2_1.Y * (double) vector2_1.Y + (double) invIb * (double) vector2_2.Y * (double) vector2_2.Y), (float) (-(double) invIa * (double) vector2_1.X * (double) vector2_1.Y - (double) invIb * (double) vector2_2.X * (double) vector2_2.Y), 0.0f, (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) vector2_1.X * (double) vector2_1.X + (double) invIb * (double) vector2_2.X * (double) vector2_2.X));
    matrix22.EY.X = matrix22.EX.Y;
    Vector2 vector2_4 = -((Matrix22) ref matrix22).Solve(vector2_3);
    Vector2 vector2_5 = position1 - vector2_4 * invMassA;
    float num6 = angle1 - invIa * Vector2Helpers.Cross(vector2_1, vector2_4);
    Vector2 vector2_6 = position2 + vector2_4 * invMassB;
    float num7 = angle2 + invIb * Vector2Helpers.Cross(vector2_2, vector2_4);
    positions[this._indexA] = vector2_5;
    angles[this._indexA] = num6;
    positions[this._indexB] = vector2_6;
    angles[this._indexB] = num7;
    return num5 <= 0.004999999888241291 && (double) num1 <= 0.034906588494777679;
  }

  public bool Equals(RevoluteJoint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.Equals((Joint) other) && this.EnableLimit == other.EnableLimit && this.EnableMotor == other.EnableMotor && MathHelper.CloseTo(this.ReferenceAngle, other.ReferenceAngle, 1E-07f) && MathHelper.CloseTo(this.LowerAngle, other.LowerAngle, 1E-07f) && MathHelper.CloseTo(this.UpperAngle, other.UpperAngle, 1E-07f) && MathHelper.CloseTo(this.MotorSpeed, other.MotorSpeed, 1E-07f) && MathHelper.CloseTo(this.MaxMotorTorque, other.MaxMotorTorque, 1E-07f);
  }

  public override bool Equals(object? obj)
  {
    return obj != null && !(obj.GetType() != this.GetType()) && this.Equals((RevoluteJoint) obj);
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new HashCode();
    hashCode.Add<EntityUid>(this.BodyAUid);
    hashCode.Add<EntityUid>(this.BodyBUid);
    return hashCode.ToHashCode();
  }

  public override Joint Clone(EntityUid uidA, EntityUid uidB)
  {
    RevoluteJoint revoluteJoint = new RevoluteJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB, this.ReferenceAngle);
    revoluteJoint.Enabled = this.Enabled;
    revoluteJoint.EnableLimit = this.EnableLimit;
    revoluteJoint.EnableMotor = this.EnableMotor;
    revoluteJoint.LowerAngle = this.LowerAngle;
    revoluteJoint.UpperAngle = this.UpperAngle;
    revoluteJoint.MaxMotorTorque = this.MaxMotorTorque;
    revoluteJoint.MotorSpeed = this.MotorSpeed;
    revoluteJoint._impulse = this._impulse;
    revoluteJoint._upperImpulse = this._upperImpulse;
    revoluteJoint._lowerImpulse = this._lowerImpulse;
    revoluteJoint._motorImpulse = this._motorImpulse;
    revoluteJoint.Breakpoint = this.Breakpoint;
    return (Joint) revoluteJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is RevoluteJoint revoluteJoint))
      return;
    revoluteJoint.Enabled = this.Enabled;
    revoluteJoint.EnableLimit = this.EnableLimit;
    revoluteJoint.EnableMotor = this.EnableMotor;
    revoluteJoint.LowerAngle = this.LowerAngle;
    revoluteJoint.UpperAngle = this.UpperAngle;
    revoluteJoint.MaxMotorTorque = this.MaxMotorTorque;
    revoluteJoint.MotorSpeed = this.MotorSpeed;
    revoluteJoint._impulse = this._impulse;
    revoluteJoint._upperImpulse = this._upperImpulse;
    revoluteJoint._lowerImpulse = this._lowerImpulse;
    revoluteJoint._motorImpulse = this._motorImpulse;
    revoluteJoint.Breakpoint = this.Breakpoint;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RevoluteJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RevoluteJoint) target1;
    if (serialization.TryCustomCopy<RevoluteJoint>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableLimit, ref target2, hookCtx, false, context))
      target2 = this.EnableLimit;
    target.EnableLimit = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableMotor, ref target3, hookCtx, false, context))
      target3 = this.EnableMotor;
    target.EnableMotor = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceAngle, ref target4, hookCtx, false, context))
      target4 = this.ReferenceAngle;
    target.ReferenceAngle = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LowerAngle, ref target5, hookCtx, false, context))
      target5 = this.LowerAngle;
    target.LowerAngle = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UpperAngle, ref target6, hookCtx, false, context))
      target6 = this.UpperAngle;
    target.UpperAngle = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MotorSpeed, ref target7, hookCtx, false, context))
      target7 = this.MotorSpeed;
    target.MotorSpeed = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxMotorTorque, ref target8, hookCtx, false, context))
      target8 = this.MaxMotorTorque;
    target.MaxMotorTorque = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RevoluteJoint target,
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
    RevoluteJoint target1 = (RevoluteJoint) target;
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
    RevoluteJoint target1 = (RevoluteJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RevoluteJoint Joint.Instantiate() => new RevoluteJoint();
}
