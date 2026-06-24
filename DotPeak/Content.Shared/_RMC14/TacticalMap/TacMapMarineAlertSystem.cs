// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacMapMarineAlertSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

public sealed class TacMapMarineAlertSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private InventorySystem _inv;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GrantTacMapAlertComponent, GotEquippedEvent>(new EntityEventRefHandler<GrantTacMapAlertComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<GrantTacMapAlertComponent, GotUnequippedEvent>(new EntityEventRefHandler<GrantTacMapAlertComponent, GotUnequippedEvent>(this.OnGotUnequipped));
    this.SubscribeLocalEvent<TacMapMarineAlertComponent, MapInitEvent>(new EntityEventRefHandler<TacMapMarineAlertComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<TacMapMarineAlertComponent, ComponentRemove>(new EntityEventRefHandler<TacMapMarineAlertComponent, ComponentRemove>(this.OnRemove));
  }

  private void OnGotEquipped(Entity<GrantTacMapAlertComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.EnsureComp<TacMapMarineAlertComponent>(args.Equipee);
  }

  private void OnGotUnequipped(Entity<GrantTacMapAlertComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE || this._inv.TryGetInventoryEntity<GrantTacMapAlertComponent>((Entity<InventoryComponent>) args.Equipee, out Entity<GrantTacMapAlertComponent> _))
      return;
    this.RemCompDeferred<TacMapMarineAlertComponent>(args.Equipee);
  }

  private void OnMapInit(Entity<TacMapMarineAlertComponent> ent, ref MapInitEvent args)
  {
    this._alerts.ShowAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnRemove(Entity<TacMapMarineAlertComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }
}
