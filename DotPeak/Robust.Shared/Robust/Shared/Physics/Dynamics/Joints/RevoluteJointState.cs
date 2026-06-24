// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.RevoluteJointState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Joints;

[NetSerializable]
[Serializable]
internal sealed class RevoluteJointState : JointState
{
  public bool EnableLimit { get; internal set; }

  public bool EnableMotor { get; internal set; }

  public float ReferenceAngle { get; internal set; }

  public float LowerAngle { get; internal set; }

  public float UpperAngle { get; internal set; }

  public float MotorSpeed { get; internal set; }

  public float MaxMotorTorque { get; internal set; }

  public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
  {
    return (Joint) new RevoluteJoint(this, entManager, owner);
  }
}
