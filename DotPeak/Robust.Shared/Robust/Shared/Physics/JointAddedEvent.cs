// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.JointAddedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;

#nullable enable
namespace Robust.Shared.Physics;

public sealed class JointAddedEvent : EntityEventArgs
{
  public readonly EntityUid OurEntity;
  public readonly EntityUid OtherEntity;

  public PhysicsComponent OurBody { get; }

  public PhysicsComponent OtherBody { get; }

  public Joint Joint { get; }

  public JointAddedEvent(
    Joint joint,
    EntityUid ourEntity,
    EntityUid otherEntity,
    PhysicsComponent ourBody,
    PhysicsComponent otherBody)
  {
    this.Joint = joint;
    this.OurEntity = ourEntity;
    this.OtherEntity = otherEntity;
    this.OurBody = ourBody;
    this.OtherBody = otherBody;
  }
}
