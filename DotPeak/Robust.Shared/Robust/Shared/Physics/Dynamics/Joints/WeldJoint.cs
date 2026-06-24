// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.WeldJoint
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

public sealed class WeldJoint : 
  Joint,
  IEquatable<WeldJoint>,
  ISerializationGenerated<WeldJoint>,
  ISerializationGenerated
{
  private float _gamma;
  private Vector3 _impulse;
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
  private Matrix33 _mass;
  [DataField("stiffness", false, 1, false, false, null)]
  public float Stiffness;
  [DataField("damping", false, 1, false, false, null)]
  public float Damping;
  [DataField("bias", false, 1, false, false, null)]
  public float Bias;
  [DataField("referenceAngle", false, 1, false, false, null)]
  public float ReferenceAngle;

  public WeldJoint()
  {
  }

  internal WeldJoint(
    EntityUid bodyA,
    EntityUid bodyB,
    Vector2 anchorA,
    Vector2 anchorB,
    float referenceAngle)
    : base(bodyA, bodyB)
  {
    this.LocalAnchorA = anchorA;
    this.LocalAnchorB = anchorB;
    this.ReferenceAngle = referenceAngle;
  }

  internal WeldJoint(EntityUid bodyAUid, EntityUid bodyBUid)
    : base(bodyAUid, bodyBUid)
  {
  }

  internal WeldJoint(WeldJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this.Stiffness = state.Stiffness;
    this.Damping = state.Damping;
    this.Bias = state.Bias;
  }

  public override JointType JointType => JointType.Weld;

  public override JointState GetState(IEntityManager entManager)
  {
    WeldJointState state = new WeldJointState();
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  public override Vector2 GetReactionForce(float invDt)
  {
    return new Vector2(this._impulse.X, this._impulse.Y) * invDt;
  }

  public override float GetReactionTorque(float invDt) => invDt * this._impulse.Z;

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
    Matrix33 matrix33;
    matrix33.EX.X = (float) ((double) invMassA + (double) invMassB + (double) this._rA.Y * (double) this._rA.Y * (double) invIa + (double) this._rB.Y * (double) this._rB.Y * (double) invIb);
    matrix33.EY.X = (float) (-(double) this._rA.Y * (double) this._rA.X * (double) invIa - (double) this._rB.Y * (double) this._rB.X * (double) invIb);
    matrix33.EZ.X = (float) (-(double) this._rA.Y * (double) invIa - (double) this._rB.Y * (double) invIb);
    matrix33.EX.Y = matrix33.EY.X;
    matrix33.EY.Y = (float) ((double) invMassA + (double) invMassB + (double) this._rA.X * (double) this._rA.X * (double) invIa + (double) this._rB.X * (double) this._rB.X * (double) invIb);
    matrix33.EZ.Y = (float) ((double) this._rA.X * (double) invIa + (double) this._rB.X * (double) invIb);
    matrix33.EX.Z = matrix33.EZ.X;
    matrix33.EY.Z = matrix33.EZ.Y;
    matrix33.EZ.Z = invIa + invIb;
    if ((double) this.Stiffness > 0.0)
    {
      ((Matrix33) ref matrix33).GetInverse22(ref this._mass);
      float num1 = invIa + invIb;
      float num2 = angle2 - angle1 - this.ReferenceAngle;
      float damping = this.Damping;
      float stiffness = this.Stiffness;
      float frameTime = data.FrameTime;
      this._gamma = frameTime * (damping + frameTime * stiffness);
      this._gamma = (double) this._gamma != 0.0 ? 1f / this._gamma : 0.0f;
      this.Bias = num2 * frameTime * stiffness * this._gamma;
      float num3 = num1 + this._gamma;
      this._mass.EZ.Z = (double) num3 != 0.0 ? 1f / num3 : 0.0f;
    }
    else if ((double) matrix33.EZ.Z == 0.0)
    {
      ((Matrix33) ref matrix33).GetInverse22(ref this._mass);
      this._gamma = 0.0f;
      this.Bias = 0.0f;
    }
    else
    {
      ((Matrix33) ref matrix33).GetSymInverse33(ref this._mass);
      this._gamma = 0.0f;
      this.Bias = 0.0f;
    }
    if (data.WarmStarting)
    {
      this._impulse *= data.DtRatio;
      Vector2 vector2 = new Vector2(this._impulse.X, this._impulse.Y);
      linearVelocity1 -= vector2 * invMassA;
      angularVelocity1 -= invIa * (Vector2Helpers.Cross(this._rA, vector2) + this._impulse.Z);
      linearVelocity2 += vector2 * invMassB;
      angularVelocity2 += invIb * (Vector2Helpers.Cross(this._rB, vector2) + this._impulse.Z);
    }
    else
      this._impulse = Vector3.Zero;
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
    Vector2 vector2_1;
    float num1;
    Vector2 vector2_2;
    float num2;
    if ((double) this.Stiffness > 0.0)
    {
      float num3 = (float) (-(double) this._mass.EZ.Z * ((double) (angularVelocity2 - angularVelocity1) + (double) this.Bias + (double) this._gamma * (double) this._impulse.Z));
      this._impulse.Z += num3;
      float num4 = angularVelocity1 - invIa * num3;
      float num5 = angularVelocity2 + invIb * num3;
      Vector2 vector2_3 = -((Matrix33) ref this._mass).Mul22(linearVelocity2 + Vector2Helpers.Cross(num5, ref this._rB) - linearVelocity1 - Vector2Helpers.Cross(num4, ref this._rA));
      this._impulse.X += vector2_3.X;
      this._impulse.Y += vector2_3.Y;
      Vector2 vector2_4 = vector2_3;
      vector2_1 = linearVelocity1 - vector2_4 * invMassA;
      num1 = num4 - invIa * Vector2Helpers.Cross(this._rA, vector2_4);
      vector2_2 = linearVelocity2 + vector2_4 * invMassB;
      num2 = num5 + invIb * Vector2Helpers.Cross(this._rB, vector2_4);
    }
    else
    {
      Vector2 vector2_5 = linearVelocity2 + Vector2Helpers.Cross(angularVelocity2, ref this._rB) - linearVelocity1 - Vector2Helpers.Cross(angularVelocity1, ref this._rA);
      float z = angularVelocity2 - angularVelocity1;
      Vector3 vector3 = -((Matrix33) ref this._mass).Mul(new Vector3(vector2_5.X, vector2_5.Y, z));
      this._impulse += vector3;
      Vector2 vector2_6 = new Vector2(vector3.X, vector3.Y);
      vector2_1 = linearVelocity1 - vector2_6 * invMassA;
      num1 = angularVelocity1 - invIa * (Vector2Helpers.Cross(this._rA, vector2_6) + vector3.Z);
      vector2_2 = linearVelocity2 + vector2_6 * invMassB;
      num2 = angularVelocity2 + invIb * (Vector2Helpers.Cross(this._rB, vector2_6) + vector3.Z);
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
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    Vector2 vector2_1 = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    Vector2 vector2_2 = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    Matrix33 matrix33;
    matrix33.EX.X = (float) ((double) invMassA + (double) invMassB + (double) vector2_1.Y * (double) vector2_1.Y * (double) invIa + (double) vector2_2.Y * (double) vector2_2.Y * (double) invIb);
    matrix33.EY.X = (float) (-(double) vector2_1.Y * (double) vector2_1.X * (double) invIa - (double) vector2_2.Y * (double) vector2_2.X * (double) invIb);
    matrix33.EZ.X = (float) (-(double) vector2_1.Y * (double) invIa - (double) vector2_2.Y * (double) invIb);
    matrix33.EX.Y = matrix33.EY.X;
    matrix33.EY.Y = (float) ((double) invMassA + (double) invMassB + (double) vector2_1.X * (double) vector2_1.X * (double) invIa + (double) vector2_2.X * (double) vector2_2.X * (double) invIb);
    matrix33.EZ.Y = (float) ((double) vector2_1.X * (double) invIa + (double) vector2_2.X * (double) invIb);
    matrix33.EX.Z = matrix33.EZ.X;
    matrix33.EY.Z = matrix33.EZ.Y;
    matrix33.EZ.Z = invIa + invIb;
    float num1;
    float num2;
    Vector2 vector2_3;
    float num3;
    Vector2 vector2_4;
    float num4;
    if ((double) this.Stiffness > 0.0)
    {
      Vector2 vector2_5 = position2 + vector2_2 - position1 - vector2_1;
      num1 = vector2_5.Length();
      num2 = 0.0f;
      Vector2 vector2_6 = -((Matrix33) ref matrix33).Solve22(vector2_5);
      vector2_3 = position1 - vector2_6 * invMassA;
      num3 = angle1 - invIa * Vector2Helpers.Cross(vector2_1, vector2_6);
      vector2_4 = position2 + vector2_6 * invMassB;
      num4 = angle2 + invIb * Vector2Helpers.Cross(vector2_2, vector2_6);
    }
    else
    {
      Vector2 vector2_7 = position2 + vector2_2 - position1 - vector2_1;
      float z = angle2 - angle1 - this.ReferenceAngle;
      num1 = vector2_7.Length();
      num2 = Math.Abs(z);
      Vector3 vector3_1 = new Vector3(vector2_7.X, vector2_7.Y, z);
      Vector3 vector3_2;
      if ((double) matrix33.EZ.Z > 0.0)
      {
        vector3_2 = -((Matrix33) ref matrix33).Solve33(vector3_1);
      }
      else
      {
        Vector2 vector2_8 = -((Matrix33) ref matrix33).Solve22(vector2_7);
        vector3_2 = new Vector3(vector2_8.X, vector2_8.Y, 0.0f);
      }
      Vector2 vector2_9 = new Vector2(vector3_2.X, vector3_2.Y);
      vector2_3 = position1 - vector2_9 * invMassA;
      num3 = angle1 - invIa * (Vector2Helpers.Cross(vector2_1, vector2_9) + vector3_2.Z);
      vector2_4 = position2 + vector2_9 * invMassB;
      num4 = angle2 + invIb * (Vector2Helpers.Cross(vector2_2, vector2_9) + vector3_2.Z);
    }
    positions[this._indexA] = vector2_3;
    angles[this._indexA] = num3;
    positions[this._indexB] = vector2_4;
    angles[this._indexB] = num4;
    return (double) num1 <= 0.004999999888241291 && (double) num2 <= 0.034906588494777679;
  }

  public override Joint Clone(EntityUid uidA, EntityUid uidB)
  {
    WeldJoint weldJoint = new WeldJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB, this.ReferenceAngle);
    weldJoint.Enabled = this.Enabled;
    weldJoint.Bias = this.Bias;
    weldJoint.Damping = this.Damping;
    weldJoint.Stiffness = this.Stiffness;
    weldJoint._impulse = this._impulse;
    weldJoint.Breakpoint = this.Breakpoint;
    return (Joint) weldJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is WeldJoint weldJoint))
      return;
    weldJoint.Enabled = this.Enabled;
    weldJoint.Bias = this.Bias;
    weldJoint.Damping = this.Damping;
    weldJoint.Stiffness = this.Stiffness;
    weldJoint._impulse = this._impulse;
    weldJoint.Breakpoint = this.Breakpoint;
  }

  public bool Equals(WeldJoint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.Equals((Joint) other) && this.Stiffness.Equals(other.Stiffness) && this.Damping.Equals(other.Damping) && this.Bias.Equals(other.Bias);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeldJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WeldJoint) target1;
    if (serialization.TryCustomCopy<WeldJoint>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Stiffness, ref target2, hookCtx, false, context))
      target2 = this.Stiffness;
    target.Stiffness = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Damping, ref target3, hookCtx, false, context))
      target3 = this.Damping;
    target.Damping = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Bias, ref target4, hookCtx, false, context))
      target4 = this.Bias;
    target.Bias = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceAngle, ref target5, hookCtx, false, context))
      target5 = this.ReferenceAngle;
    target.ReferenceAngle = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeldJoint target,
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
    WeldJoint target1 = (WeldJoint) target;
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
    WeldJoint target1 = (WeldJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WeldJoint Joint.Instantiate() => new WeldJoint();
}
