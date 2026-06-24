// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wieldable.UnwieldAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Wieldable;

[ByRefEvent]
public record struct UnwieldAttemptEvent(EntityUid User, EntityUid Wielded, bool Cancelled = false) : 
  IInventoryRelayEvent
{
  public string? Message = (string) null;

  public EntityUid User { get; set; } = User;

  public EntityUid Wielded { get; set; } = Wielded;

  public bool Cancelled { get; set; } = Cancelled;

  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

  public void Cancel() => this.Cancelled = true;
}
