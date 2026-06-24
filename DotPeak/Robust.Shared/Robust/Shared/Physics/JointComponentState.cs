// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.JointComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Physics;

[NetSerializable]
[Serializable]
public sealed class JointComponentState : ComponentState
{
  public NetEntity? Relay;
  public Dictionary<string, JointState> Joints;

  public JointComponentState(NetEntity? relay, Dictionary<string, JointState> joints)
  {
    this.Relay = relay;
    this.Joints = joints;
  }
}
