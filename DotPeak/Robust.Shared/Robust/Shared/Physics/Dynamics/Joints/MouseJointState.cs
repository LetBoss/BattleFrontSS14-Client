// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.MouseJointState
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
internal sealed class MouseJointState : JointState
{
  public float MaxForce { get; internal set; }

  public float Stiffness { get; internal set; }

  public float Damping { get; internal set; }

  public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
  {
    return (Joint) new MouseJoint(this, entManager, owner);
  }
}
