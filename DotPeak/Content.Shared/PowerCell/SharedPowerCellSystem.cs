// Decompiled with JetBrains decompiler
// Type: Content.Shared.PowerCell.SharedPowerCellSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.PowerCell.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.PowerCell;

public abstract class SharedPowerCellSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PowerCellDrawComponent, MapInitEvent>(new EntityEventRefHandler<PowerCellDrawComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<PowerCellSlotComponent, RejuvenateEvent>(new ComponentEventHandler<PowerCellSlotComponent, RejuvenateEvent>(this.OnRejuvenate));
    this.SubscribeLocalEvent<PowerCellSlotComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PowerCellSlotComponent, EntInsertedIntoContainerMessage>(this.OnCellInserted));
    this.SubscribeLocalEvent<PowerCellSlotComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PowerCellSlotComponent, EntRemovedFromContainerMessage>(this.OnCellRemoved));
    this.SubscribeLocalEvent<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>(this.OnCellInsertAttempt));
  }

  private void OnMapInit(Entity<PowerCellDrawComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextUpdateTime = this.Timing.CurTime + ent.Comp.Delay;
  }

  private void OnRejuvenate(EntityUid uid, PowerCellSlotComponent component, RejuvenateEvent args)
  {
    ItemSlot itemSlot;
    if (!this._itemSlots.TryGetSlot(uid, component.CellSlotId, out itemSlot))
      return;
    EntityUid? nullable = itemSlot.Item;
    if (!nullable.HasValue)
      return;
    nullable = itemSlot.Item;
    this.RaiseLocalEvent<RejuvenateEvent>(nullable.Value, args);
  }

  private void OnCellInsertAttempt(
    EntityUid uid,
    PowerCellSlotComponent component,
    ContainerIsInsertingAttemptEvent args)
  {
    if (!component.Initialized || args.Container.ID != component.CellSlotId || this.HasComp<PowerCellComponent>(args.EntityUid))
      return;
    args.Cancel();
  }

  private void OnCellInserted(
    EntityUid uid,
    PowerCellSlotComponent component,
    EntInsertedIntoContainerMessage args)
  {
    if (!component.Initialized || args.Container.ID != component.CellSlotId)
      return;
    this._appearance.SetData(uid, (Enum) PowerCellSlotVisuals.Enabled, (object) true);
    this.RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(false));
  }

  protected virtual void OnCellRemoved(
    EntityUid uid,
    PowerCellSlotComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != component.CellSlotId)
      return;
    this._appearance.SetData(uid, (Enum) PowerCellSlotVisuals.Enabled, (object) false);
    this.RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(true));
  }

  public void SetDrawEnabled(Entity<PowerCellDrawComponent?> ent, bool enabled)
  {
    if (!this.Resolve<PowerCellDrawComponent>((EntityUid) ent, ref ent.Comp, false) || ent.Comp.Enabled == enabled)
      return;
    if (enabled)
      ent.Comp.NextUpdateTime = this.Timing.CurTime;
    ent.Comp.Enabled = enabled;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public abstract bool HasActivatableCharge(
    EntityUid uid,
    PowerCellDrawComponent? battery = null,
    PowerCellSlotComponent? cell = null,
    EntityUid? user = null);

  public abstract bool HasDrawCharge(
    EntityUid uid,
    PowerCellDrawComponent? battery = null,
    PowerCellSlotComponent? cell = null,
    EntityUid? user = null);
}
