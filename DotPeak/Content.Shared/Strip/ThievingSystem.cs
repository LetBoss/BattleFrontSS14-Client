// Decompiled with JetBrains decompiler
// Type: Content.Shared.Strip.ThievingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Inventory;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Strip;

public sealed class ThievingSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alertsSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ThievingComponent, BeforeStripEvent>(new ComponentEventHandler<ThievingComponent, BeforeStripEvent>(this.OnBeforeStrip));
    this.SubscribeLocalEvent<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>((ComponentEventHandler<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>) ((e, c, ev) => this.OnBeforeStrip(e, c, ev.Args)));
    this.SubscribeLocalEvent<ThievingComponent, ToggleThievingEvent>(new EntityEventRefHandler<ThievingComponent, ToggleThievingEvent>(this.OnToggleStealthy));
    this.SubscribeLocalEvent<ThievingComponent, ComponentInit>(new EntityEventRefHandler<ThievingComponent, ComponentInit>(this.OnCompInit));
    this.SubscribeLocalEvent<ThievingComponent, ComponentRemove>(new EntityEventRefHandler<ThievingComponent, ComponentRemove>(this.OnCompRemoved));
  }

  private void OnBeforeStrip(EntityUid uid, ThievingComponent component, BeforeStripEvent args)
  {
    args.Stealth |= component.Stealthy;
    if (!args.Stealth)
      return;
    BeforeStripEvent beforeStripEvent = args;
    beforeStripEvent.Additive = beforeStripEvent.Additive - component.StripTimeReduction;
  }

  private void OnCompInit(Entity<ThievingComponent> entity, ref ComponentInit args)
  {
    this._alertsSystem.ShowAlert((EntityUid) entity, entity.Comp.StealthyAlertProtoId, new short?((short) 1));
  }

  private void OnCompRemoved(Entity<ThievingComponent> entity, ref ComponentRemove args)
  {
    this._alertsSystem.ClearAlert((EntityUid) entity, entity.Comp.StealthyAlertProtoId);
  }

  private void OnToggleStealthy(Entity<ThievingComponent> ent, ref ToggleThievingEvent args)
  {
    if (args.Handled)
      return;
    ent.Comp.Stealthy = !ent.Comp.Stealthy;
    this._alertsSystem.ShowAlert(ent.Owner, ent.Comp.StealthyAlertProtoId, new short?((short) ent.Comp.Stealthy));
    this.DirtyField<ThievingComponent>(ent.AsNullable(), "Stealthy");
    args.Handled = true;
  }
}
