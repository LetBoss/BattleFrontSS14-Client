// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.GetFireProtectionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable disable
namespace Content.Shared.Atmos;

[ByRefEvent]
public sealed class GetFireProtectionEvent : EntityEventArgs, IInventoryRelayEvent
{
  public float Multiplier;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public GetFireProtectionEvent() => this.Multiplier = 1f;

  public void Reduce(float by)
  {
    this.Multiplier -= by;
    this.Multiplier = MathF.Max(this.Multiplier, 0.0f);
  }
}
