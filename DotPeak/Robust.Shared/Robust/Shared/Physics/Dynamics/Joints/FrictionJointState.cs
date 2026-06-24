// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.FrictionJointState
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
public sealed class FrictionJointState : JointState
{
  public float MaxForce { get; }

  public float MaxTorque { get; }

  public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
  {
    return (Joint) new FrictionJoint(this, entManager, owner);
  }
}
