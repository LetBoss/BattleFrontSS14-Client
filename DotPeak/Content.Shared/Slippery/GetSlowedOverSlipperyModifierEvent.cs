// Decompiled with JetBrains decompiler
// Type: Content.Shared.Slippery.GetSlowedOverSlipperyModifierEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Slippery;

[ByRefEvent]
public record struct GetSlowedOverSlipperyModifierEvent : IInventoryRelayEvent
{
  public float SlowdownModifier;

  public GetSlowedOverSlipperyModifierEvent() => this.SlowdownModifier = 1f;

  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<float>.Default.GetHashCode(this.SlowdownModifier);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetSlowedOverSlipperyModifierEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.SlowdownModifier, other.SlowdownModifier);
  }
}
