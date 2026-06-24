// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Components.PhysicsComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Components;

[NetSerializable]
[Serializable]
public sealed class PhysicsComponentState : IComponentState
{
  public bool CanCollide;
  public bool SleepingAllowed;
  public bool FixedRotation;
  public BodyStatus Status;
  public Vector2 LinearVelocity;
  public float AngularVelocity;
  public BodyType BodyType;
  public float Friction;
  public float LinearDamping;
  public float AngularDamping;
  public Vector2 Force;
  public float Torque;

  public PhysicsComponentState()
  {
  }

  public PhysicsComponentState(PhysicsComponentState existing)
  {
    this.CanCollide = existing.CanCollide;
    this.SleepingAllowed = existing.SleepingAllowed;
    this.FixedRotation = existing.FixedRotation;
    this.Status = existing.Status;
    this.LinearVelocity = existing.LinearVelocity;
    this.AngularVelocity = existing.AngularVelocity;
    this.BodyType = existing.BodyType;
    this.Friction = existing.Friction;
    this.LinearDamping = existing.LinearDamping;
    this.AngularDamping = existing.AngularDamping;
    this.Force = existing.Force;
    this.Torque = existing.Torque;
  }
}
