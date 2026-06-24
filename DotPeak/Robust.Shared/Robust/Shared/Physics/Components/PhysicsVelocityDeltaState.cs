// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Components.PhysicsVelocityDeltaState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Components;

[NetSerializable]
[Serializable]
public record struct PhysicsVelocityDeltaState : 
  IComponentDeltaState<PhysicsComponentState>,
  IComponentDeltaState,
  IComponentState
{
  public Vector2 LinearVelocity;
  public float AngularVelocity;

  public void ApplyToFullState(PhysicsComponentState fullState)
  {
    fullState.LinearVelocity = this.LinearVelocity;
    fullState.AngularVelocity = this.AngularVelocity;
  }

  public PhysicsComponentState CreateNewFullState(PhysicsComponentState fullState)
  {
    return new PhysicsComponentState(fullState)
    {
      LinearVelocity = this.LinearVelocity,
      AngularVelocity = this.AngularVelocity
    };
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<Vector2>.Default.GetHashCode(this.LinearVelocity) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.AngularVelocity);
  }

  [CompilerGenerated]
  public readonly bool Equals(PhysicsVelocityDeltaState other)
  {
    return EqualityComparer<Vector2>.Default.Equals(this.LinearVelocity, other.LinearVelocity) && EqualityComparer<float>.Default.Equals(this.AngularVelocity, other.AngularVelocity);
  }
}
