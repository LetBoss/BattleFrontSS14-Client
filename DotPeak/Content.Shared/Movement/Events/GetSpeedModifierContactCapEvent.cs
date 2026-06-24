// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Events.GetSpeedModifierContactCapEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Movement.Events;

[ByRefEvent]
public record struct GetSpeedModifierContactCapEvent : IInventoryRelayEvent
{
  public float MaxSprintSlowdown;
  public float MaxWalkSlowdown;

  public GetSpeedModifierContactCapEvent()
  {
    this.MaxSprintSlowdown = 0.0f;
    this.MaxWalkSlowdown = 0.0f;
  }

  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

  public void SetIfMax(float valueSprint, float valueWalk)
  {
    this.MaxSprintSlowdown = MathF.Max(this.MaxSprintSlowdown, valueSprint);
    this.MaxWalkSlowdown = MathF.Max(this.MaxWalkSlowdown, valueWalk);
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<float>.Default.GetHashCode(this.MaxSprintSlowdown) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.MaxWalkSlowdown);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetSpeedModifierContactCapEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.MaxSprintSlowdown, other.MaxSprintSlowdown) && EqualityComparer<float>.Default.Equals(this.MaxWalkSlowdown, other.MaxWalkSlowdown);
  }
}
