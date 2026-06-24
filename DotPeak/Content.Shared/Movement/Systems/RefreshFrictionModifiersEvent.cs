// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.RefreshFrictionModifiersEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct RefreshFrictionModifiersEvent : IInventoryRelayEvent
{
  public float Friction;
  public float FrictionNoInput;
  public float Acceleration;

  public void ModifyFriction(float friction, float noInput)
  {
    this.Friction *= friction;
    this.FrictionNoInput *= noInput;
  }

  public void ModifyFriction(float friction) => this.ModifyFriction(friction, friction);

  public void ModifyAcceleration(float acceleration) => this.Acceleration *= acceleration;

  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<float>.Default.GetHashCode(this.Friction) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.FrictionNoInput)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Acceleration);
  }

  [CompilerGenerated]
  public readonly bool Equals(RefreshFrictionModifiersEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.Friction, other.Friction) && EqualityComparer<float>.Default.Equals(this.FrictionNoInput, other.FrictionNoInput) && EqualityComparer<float>.Default.Equals(this.Acceleration, other.Acceleration);
  }
}
