// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.HUD.Systems.HolocardScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.HUD.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Medical.HUD.Systems;

public sealed class HolocardScannerSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<InventoryComponent, HolocardScanEvent>(new EntityEventRefHandler<InventoryComponent, HolocardScanEvent>(this._inventory.RelayEvent<HolocardScanEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<HolocardScannerComponent>>(new EntityEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<HolocardScannerComponent>>(this._inventory.RelayEvent<RefreshEquipmentHudEvent<HolocardScannerComponent>>));
    this.SubscribeLocalEvent<HolocardScannerComponent, HolocardScanEvent>(new EntityEventRefHandler<HolocardScannerComponent, HolocardScanEvent>(this.OnHolocardScanAttempt));
    this.SubscribeLocalEvent<HolocardScannerComponent, InventoryRelayedEvent<HolocardScanEvent>>(new EntityEventRefHandler<HolocardScannerComponent, InventoryRelayedEvent<HolocardScanEvent>>(this.OnRelayedHolocardScanAttempt));
  }

  private void OnHolocardScanAttempt(
    Entity<HolocardScannerComponent> ent,
    ref HolocardScanEvent args)
  {
    args.CanScan = true;
  }

  private void OnRelayedHolocardScanAttempt(
    Entity<HolocardScannerComponent> ent,
    ref InventoryRelayedEvent<HolocardScanEvent> args)
  {
    args.Args.CanScan = true;
  }
}
