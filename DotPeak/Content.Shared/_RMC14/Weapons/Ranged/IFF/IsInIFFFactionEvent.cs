// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.IFF.IsInIFFFactionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable disable
namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

[ByRefEvent]
public record struct IsInIFFFactionEvent(EntProtoId Faction, bool InFaction = false, SlotFlags TargetSlots = SlotFlags.IDCARD) : 
  IInventoryRelayEvent
{
  public void TryHandle(EntProtoId? id)
  {
    if (this.InFaction)
      return;
    EntProtoId? nullable = id;
    EntProtoId faction = this.Faction;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == faction ? 1 : 0) : 0) == 0)
      return;
    this.InFaction = true;
  }
}
