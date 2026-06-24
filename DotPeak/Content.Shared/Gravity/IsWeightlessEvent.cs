// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gravity.IsWeightlessEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Gravity;

[ByRefEvent]
public record struct IsWeightlessEvent(EntityUid Entity, bool IsWeightless = false, bool Handled = false) : 
  IInventoryRelayEvent
{
  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
