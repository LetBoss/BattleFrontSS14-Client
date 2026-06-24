// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Joints.JointState
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
public abstract class JointState
{
  public string ID { get; internal set; }

  public bool Enabled { get; internal set; }

  public bool CollideConnected { get; internal set; }

  public NetEntity UidA { get; internal set; }

  public NetEntity UidB { get; internal set; }

  public Vector2 LocalAnchorA { get; internal set; }

  public Vector2 LocalAnchorB { get; internal set; }

  public float Breakpoint { get; internal set; }

  public abstract Joint GetJoint(IEntityManager entManager, EntityUid owner);
}
