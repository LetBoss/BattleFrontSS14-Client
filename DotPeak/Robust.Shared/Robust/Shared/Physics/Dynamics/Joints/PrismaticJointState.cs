// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.PrismaticJointState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Joints;

[NetSerializable]
[Serializable]
internal sealed class PrismaticJointState : JointState
{
  public Vector2 LocalAxisA;
  public float ReferenceAngle;
  public bool EnableLimit;
  public float LowerTranslation;
  public float UpperTranslation;
  public bool EnableMotor;
  public float MaxMotorForce;
  public float MotorSpeed;

  public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
  {
    return (Joint) new PrismaticJoint(this, entManager, owner);
  }
}
