// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.PrismaticJoint
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

public sealed class PrismaticJoint : 
  Joint,
  IEquatable<PrismaticJoint>,
  ISerializationGenerated<PrismaticJoint>,
  ISerializationGenerated
{
  private Vector2 _localAxisA;
  [DataField("referenceANgle", false, 1, false, false, null)]
  public float ReferenceAngle;
  [DataField("enableLimit", false, 1, false, false, null)]
  public bool EnableLimit;
  [DataField("lowerTranslation", false, 1, false, false, null)]
  public float LowerTranslation;
  [DataField("upperTranslation", false, 1, false, false, null)]
  public float UpperTranslation;
  [DataField("enableMotor", false, 1, false, false, null)]
  public bool EnableMotor;
  [DataField("maxMotorForce", false, 1, false, false, null)]
  public float MaxMotorForce;
  [DataField("motorSpeed", false, 1, false, false, null)]
  public float MotorSpeed;
  internal Vector2 _localXAxisA;
  internal Vector2 _localYAxisA;
  private Vector2 _impulse;
  private float _motorImpulse;
  private float _lowerImpulse;
  private float _upperImpulse;
  private int _indexA;
  private int _indexB;
  private Vector2 _localCenterA;
  private Vector2 _localCenterB;
  private float _invMassA;
  private float _invMassB;
  private float _invIA;
  private float _invIB;
  private Vector2 _axis;
  private Vector2 _perp;
  private float _s1;
  private float _s2;
  private float _a1;
  private float _a2;
  private Matrix22 _K;
  private float _translation;
  private float _axialMass;

  [DataField("localAxisA", false, 1, false, false, null)]
  public Vector2 LocalAxisA
  {
    get => this._localAxisA;
    set
    {
      this._localAxisA = value;
      this._localXAxisA = Vector2Helpers.Normalized(value);
      this._localYAxisA = Vector2Helpers.Cross(1f, ref this._localXAxisA);
    }
  }

  public PrismaticJoint()
  {
  }

  internal PrismaticJoint(EntityUid bodyAUid, EntityUid bodyBUid)
    : base(bodyAUid, bodyBUid)
  {
    this.LocalAxisA = new Vector2(1f, 0.0f);
  }

  public PrismaticJoint(
    EntityUid bodyAUid,
    EntityUid bodyBUid,
    Vector2 anchorA,
    Vector2 anchorB,
    Vector2 axis,
    float referenceAngle)
    : base(bodyAUid, bodyBUid)
  {
    this.LocalAnchorA = anchorA;
    this.LocalAnchorB = anchorB;
    this.LocalAxisA = axis;
    this.ReferenceAngle = referenceAngle;
  }

  internal PrismaticJoint(PrismaticJointState state, IEntityManager entManager, EntityUid owner)
    : base((JointState) state, entManager, owner)
  {
    this.LocalAxisA = state.LocalAxisA;
    this.ReferenceAngle = state.ReferenceAngle;
    this.EnableLimit = state.EnableLimit;
    this.LowerTranslation = state.LowerTranslation;
    this.UpperTranslation = state.UpperTranslation;
    this.EnableMotor = state.EnableMotor;
    this.MaxMotorForce = state.MaxMotorForce;
    this.MotorSpeed = state.MotorSpeed;
  }

  public override JointType JointType => JointType.Prismatic;

  public override JointState GetState(IEntityManager entManager)
  {
    PrismaticJointState prismaticJointState = new PrismaticJointState();
    prismaticJointState.LocalAnchorA = this.LocalAnchorA;
    prismaticJointState.LocalAnchorB = this.LocalAnchorB;
    PrismaticJointState state = prismaticJointState;
    this.GetState((JointState) state, entManager);
    return (JointState) state;
  }

  public override Vector2 GetReactionForce(float invDt)
  {
    return (this._perp * this._impulse.X + this._axis * (this._motorImpulse + this._lowerImpulse - this._upperImpulse)) * invDt;
  }

  public override float GetReactionTorque(float invDt) => invDt * this._impulse.Y;

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
    Vector2 vector2_1 = Transform.Mul(in quaternion2D1, this.LocalAnchorA - this._localCenterA);
    Vector2 vector2_2 = Transform.Mul(in quaternion2D2, this.LocalAnchorB - this._localCenterB);
    Vector2 vector2_3 = position1;
    Vector2 vector2_4 = position2 - vector2_3 + vector2_2 - vector2_1;
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    this._axis = Transform.Mul(in quaternion2D1, in this._localXAxisA);
    this._a1 = Vector2Helpers.Cross(vector2_4 + vector2_1, this._axis);
    this._a2 = Vector2Helpers.Cross(vector2_2, this._axis);
    this._axialMass = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) this._a1 * (double) this._a1 + (double) invIb * (double) this._a2 * (double) this._a2);
    if ((double) this._axialMass > 0.0)
      this._axialMass = 1f / this._axialMass;
    this._perp = Transform.Mul(in quaternion2D1, in this._localYAxisA);
    this._s1 = Vector2Helpers.Cross(vector2_4 + vector2_1, this._perp);
    this._s2 = Vector2Helpers.Cross(vector2_2, this._perp);
    float x = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) this._s1 * (double) this._s1 + (double) invIb * (double) this._s2 * (double) this._s2);
    float num1 = (float) ((double) invIa * (double) this._s1 + (double) invIb * (double) this._s2);
    float y = invIa + invIb;
    if ((double) y == 0.0)
      y = 1f;
    this._K = new Matrix22(new Vector2(x, num1), new Vector2(num1, y));
    if (this.EnableLimit)
    {
      this._translation = Vector2.Dot(this._axis, vector2_4);
    }
    else
    {
      this._lowerImpulse = 0.0f;
      this._upperImpulse = 0.0f;
    }
    if (!this.EnableMotor)
      this._motorImpulse = 0.0f;
    if (data.WarmStarting)
    {
      this._impulse *= data.DtRatio;
      this._motorImpulse *= data.DtRatio;
      this._lowerImpulse *= data.DtRatio;
      this._upperImpulse *= data.DtRatio;
      float num2 = this._motorImpulse + this._lowerImpulse - this._upperImpulse;
      Vector2 vector2_5 = this._perp * this._impulse.X + this._axis * num2;
      float num3 = (float) ((double) this._impulse.X * (double) this._s1 + (double) this._impulse.Y + (double) num2 * (double) this._a1);
      float num4 = (float) ((double) this._impulse.X * (double) this._s2 + (double) this._impulse.Y + (double) num2 * (double) this._a2);
      linearVelocity1 -= vector2_5 * invMassA;
      angularVelocity1 -= invIa * num3;
      linearVelocity2 += vector2_5 * invMassB;
      angularVelocity2 += invIb * num4;
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
    Vector2 vector2_1 = linearVelocities[offset + this._indexA];
    float num1 = angularVelocities[offset + this._indexA];
    Vector2 vector2_2 = linearVelocities[offset + this._indexB];
    float num2 = angularVelocities[offset + this._indexB];
    float invMassA = this._invMassA;
    float invMassB = this._invMassB;
    float invIa = this._invIA;
    float invIb = this._invIB;
    if (this.EnableMotor)
    {
      float num3 = this._axialMass * (this.MotorSpeed - (float) ((double) Vector2.Dot(this._axis, vector2_2 - vector2_1) + (double) this._a2 * (double) num2 - (double) this._a1 * (double) num1));
      float motorImpulse = this._motorImpulse;
      float max = data.FrameTime * this.MaxMotorForce;
      this._motorImpulse = Math.Clamp(this._motorImpulse + num3, -max, max);
      float num4 = this._motorImpulse - motorImpulse;
      Vector2 vector2_3 = this._axis * num4;
      float num5 = num4 * this._a1;
      float num6 = num4 * this._a2;
      vector2_1 -= vector2_3 * invMassA;
      num1 -= invIa * num5;
      vector2_2 += vector2_3 * invMassB;
      num2 += invIb * num6;
    }
    if (this.EnableLimit)
    {
      float x1 = this._translation - this.LowerTranslation;
      float num7 = (float) (-(double) this._axialMass * ((double) Vector2.Dot(this._axis, vector2_2 - vector2_1) + (double) this._a2 * (double) num2 - (double) this._a1 * (double) num1 + (double) MathF.Max(x1, 0.0f) * (double) data.InvDt));
      float lowerImpulse = this._lowerImpulse;
      this._lowerImpulse = MathF.Max(this._lowerImpulse + num7, 0.0f);
      float num8 = this._lowerImpulse - lowerImpulse;
      Vector2 vector2_4 = this._axis * num8;
      float num9 = num8 * this._a1;
      float num10 = num8 * this._a2;
      Vector2 vector2_5 = vector2_1 - vector2_4 * invMassA;
      float num11 = num1 - invIa * num9;
      Vector2 vector2_6 = vector2_2 + vector2_4 * invMassB;
      float num12 = num2 + invIb * num10;
      float x2 = this.UpperTranslation - this._translation;
      float num13 = (float) (-(double) this._axialMass * ((double) Vector2.Dot(this._axis, vector2_5 - vector2_6) + (double) this._a1 * (double) num11 - (double) this._a2 * (double) num12 + (double) MathF.Max(x2, 0.0f) * (double) data.InvDt));
      float upperImpulse = this._upperImpulse;
      this._upperImpulse = MathF.Max(this._upperImpulse + num13, 0.0f);
      float num14 = this._upperImpulse - upperImpulse;
      Vector2 vector2_7 = this._axis * num14;
      float num15 = num14 * this._a1;
      float num16 = num14 * this._a2;
      vector2_1 = vector2_5 + vector2_7 * invMassA;
      num1 = num11 + invIa * num15;
      vector2_2 = vector2_6 - vector2_7 * invMassB;
      num2 = num12 - invIb * num16;
    }
    Vector2 vector2_8;
    vector2_8.X = (float) ((double) Vector2.Dot(this._perp, vector2_2 - vector2_1) + (double) this._s2 * (double) num2 - (double) this._s1 * (double) num1);
    vector2_8.Y = num2 - num1;
    Vector2 vector2_9 = ((Matrix22) ref this._K).Solve(-vector2_8);
    this._impulse += vector2_9;
    Vector2 vector2_10 = this._perp * vector2_9.X;
    float num17 = vector2_9.X * this._s1 + vector2_9.Y;
    float num18 = vector2_9.X * this._s2 + vector2_9.Y;
    Vector2 vector2_11 = vector2_1 - vector2_10 * invMassA;
    float num19 = num1 - invIa * num17;
    Vector2 vector2_12 = vector2_2 + vector2_10 * invMassB;
    float num20 = num2 + invIb * num18;
    linearVelocities[offset + this._indexA] = vector2_11;
    angularVelocities[offset + this._indexA] = num19;
    linearVelocities[offset + this._indexB] = vector2_12;
    angularVelocities[offset + this._indexB] = num20;
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
    Vector2 vector2_3 = position2 + vector2_2 - position1 - vector2_1;
    Vector2 vector2_4 = Transform.Mul(in quaternion2D1, in this._localXAxisA);
    float num1 = Vector2Helpers.Cross(vector2_3 + vector2_1, vector2_4);
    float num2 = Vector2Helpers.Cross(vector2_2, vector2_4);
    Vector2 vector2_5 = Transform.Mul(in quaternion2D1, in this._localYAxisA);
    float num3 = Vector2Helpers.Cross(vector2_3 + vector2_1, vector2_5);
    float num4 = Vector2Helpers.Cross(vector2_2, vector2_5);
    Vector2 vector2_6;
    vector2_6.X = Vector2.Dot(vector2_5, vector2_3);
    vector2_6.Y = angle2 - angle1 - this.ReferenceAngle;
    float x1 = MathF.Abs(vector2_6.X);
    float num5 = MathF.Abs(vector2_6.Y);
    bool flag = false;
    float num6 = 0.0f;
    if (this.EnableLimit)
    {
      float x2 = Vector2.Dot(vector2_4, vector2_3);
      if ((double) MathF.Abs(this.UpperTranslation - this.LowerTranslation) < 0.0099999997764825821)
      {
        num6 = x2;
        x1 = MathF.Max(x1, MathF.Abs(x2));
        flag = true;
      }
      else if ((double) x2 <= (double) this.LowerTranslation)
      {
        num6 = MathF.Min(x2 - this.LowerTranslation, 0.0f);
        x1 = MathF.Max(x1, this.LowerTranslation - x2);
        flag = true;
      }
      else if ((double) x2 >= (double) this.UpperTranslation)
      {
        num6 = MathF.Max(x2 - this.UpperTranslation, 0.0f);
        x1 = MathF.Max(x1, x2 - this.UpperTranslation);
        flag = true;
      }
    }
    Vector3 vector3_1;
    if (flag)
    {
      float x3 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num3 * (double) num3 + (double) invIb * (double) num4 * (double) num4);
      float num7 = (float) ((double) invIa * (double) num3 + (double) invIb * (double) num4);
      float num8 = (float) ((double) invIa * (double) num3 * (double) num1 + (double) invIb * (double) num4 * (double) num2);
      float y = invIa + invIb;
      if ((double) y == 0.0)
        y = 1f;
      float num9 = (float) ((double) invIa * (double) num1 + (double) invIb * (double) num2);
      float z = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num1 * (double) num1 + (double) invIb * (double) num2 * (double) num2);
      Matrix33 matrix33;
      // ISSUE: explicit constructor call
      ((Matrix33) ref matrix33).\u002Ector(new Vector3(x3, num7, num8), new Vector3(num7, y, num9), new Vector3(num8, num9, z));
      Vector3 vector3_2;
      vector3_2.X = vector2_6.X;
      vector3_2.Y = vector2_6.Y;
      vector3_2.Z = num6;
      vector3_1 = ((Matrix33) ref matrix33).Solve33(-vector3_2);
    }
    else
    {
      float num10 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num3 * (double) num3 + (double) invIb * (double) num4 * (double) num4);
      float num11 = (float) ((double) invIa * (double) num3 + (double) invIb * (double) num4);
      float num12 = invIa + invIb;
      if ((double) num12 == 0.0)
        num12 = 1f;
      Matrix22 matrix22;
      // ISSUE: explicit constructor call
      ((Matrix22) ref matrix22).\u002Ector(num10, num11, num11, num12);
      Vector2 vector2_7 = ((Matrix22) ref matrix22).Solve(-vector2_6);
      vector3_1.X = vector2_7.X;
      vector3_1.Y = vector2_7.Y;
      vector3_1.Z = 0.0f;
    }
    Vector2 vector2_8 = vector2_5 * vector3_1.X + vector2_4 * vector3_1.Z;
    float num13 = (float) ((double) vector3_1.X * (double) num3 + (double) vector3_1.Y + (double) vector3_1.Z * (double) num1);
    float num14 = (float) ((double) vector3_1.X * (double) num4 + (double) vector3_1.Y + (double) vector3_1.Z * (double) num2);
    Vector2 vector2_9 = position1 - vector2_8 * invMassA;
    float num15 = angle1 - invIa * num13;
    Vector2 vector2_10 = position2 + vector2_8 * invMassB;
    float num16 = angle2 + invIb * num14;
    positions[this._indexA] = vector2_9;
    angles[this._indexA] = num15;
    positions[this._indexB] = vector2_10;
    angles[this._indexB] = num16;
    return (double) x1 <= 0.004999999888241291 && (double) num5 <= 0.034906588494777679;
  }

  public override Joint Clone(EntityUid uidA, EntityUid uidB)
  {
    PrismaticJoint prismaticJoint = new PrismaticJoint(uidA, uidB, this.LocalAnchorA, this.LocalAnchorB, this.LocalAxisA, this.ReferenceAngle);
    prismaticJoint.EnableLimit = this.EnableLimit;
    prismaticJoint.LowerTranslation = this.LowerTranslation;
    prismaticJoint.UpperTranslation = this.UpperTranslation;
    prismaticJoint.EnableMotor = this.EnableMotor;
    prismaticJoint.MaxMotorForce = this.MaxMotorForce;
    prismaticJoint.MotorSpeed = this.MotorSpeed;
    prismaticJoint._impulse = this._impulse;
    prismaticJoint._lowerImpulse = this._lowerImpulse;
    prismaticJoint._upperImpulse = this._upperImpulse;
    prismaticJoint._motorImpulse = this._motorImpulse;
    prismaticJoint.Breakpoint = this.Breakpoint;
    return (Joint) prismaticJoint;
  }

  public override void CopyTo(Joint original)
  {
    if (!(original is PrismaticJoint prismaticJoint))
      return;
    prismaticJoint.EnableLimit = this.EnableLimit;
    prismaticJoint.LowerTranslation = this.LowerTranslation;
    prismaticJoint.UpperTranslation = this.UpperTranslation;
    prismaticJoint.EnableMotor = this.EnableMotor;
    prismaticJoint.MaxMotorForce = this.MaxMotorForce;
    prismaticJoint.MotorSpeed = this.MotorSpeed;
    prismaticJoint._impulse = this._impulse;
    prismaticJoint._lowerImpulse = this._lowerImpulse;
    prismaticJoint._upperImpulse = this._upperImpulse;
    prismaticJoint._motorImpulse = this._motorImpulse;
    prismaticJoint.Breakpoint = this.Breakpoint;
  }

  public bool Equals(PrismaticJoint? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.Equals((Joint) other) && this.EnableLimit.Equals(other.EnableLimit) && this.EnableMotor.Equals(other.EnableMotor) && Vector2Helpers.EqualsApprox(this.LocalAxisA, other.LocalAxisA) && MathHelper.CloseTo(this.ReferenceAngle, other.ReferenceAngle, 1E-07f) && MathHelper.CloseTo(this.LowerTranslation, other.LowerTranslation, 1E-07f) && MathHelper.CloseTo(this.UpperTranslation, other.UpperTranslation, 1E-07f) && MathHelper.CloseTo(this.MaxMotorForce, other.MaxMotorForce, 1E-07f) && MathHelper.CloseTo(this.MotorSpeed, other.MotorSpeed, 1E-07f);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrismaticJoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Joint target1 = (Joint) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PrismaticJoint) target1;
    if (serialization.TryCustomCopy<PrismaticJoint>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.LocalAxisA, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.LocalAxisA, hookCtx, context);
    target.LocalAxisA = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceAngle, ref target3, hookCtx, false, context))
      target3 = this.ReferenceAngle;
    target.ReferenceAngle = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableLimit, ref target4, hookCtx, false, context))
      target4 = this.EnableLimit;
    target.EnableLimit = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LowerTranslation, ref target5, hookCtx, false, context))
      target5 = this.LowerTranslation;
    target.LowerTranslation = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UpperTranslation, ref target6, hookCtx, false, context))
      target6 = this.UpperTranslation;
    target.UpperTranslation = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableMotor, ref target7, hookCtx, false, context))
      target7 = this.EnableMotor;
    target.EnableMotor = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxMotorForce, ref target8, hookCtx, false, context))
      target8 = this.MaxMotorForce;
    target.MaxMotorForce = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MotorSpeed, ref target9, hookCtx, false, context))
      target9 = this.MotorSpeed;
    target.MotorSpeed = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrismaticJoint target,
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
    PrismaticJoint target1 = (PrismaticJoint) target;
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
    PrismaticJoint target1 = (PrismaticJoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PrismaticJoint Joint.Instantiate() => new PrismaticJoint();
}
