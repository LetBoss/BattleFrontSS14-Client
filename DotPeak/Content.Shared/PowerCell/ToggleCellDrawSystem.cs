// Decompiled with JetBrains decompiler
// Type: Content.Shared.PowerCell.ToggleCellDrawSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.PowerCell.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.PowerCell;

public sealed class ToggleCellDrawSystem : EntitySystem
{
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedPowerCellSystem _cell;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ToggleCellDrawComponent, MapInitEvent>(new EntityEventRefHandler<ToggleCellDrawComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ToggleCellDrawComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<ToggleCellDrawComponent, ItemToggleActivateAttemptEvent>(this.OnActivateAttempt));
    this.SubscribeLocalEvent<ToggleCellDrawComponent, ItemToggledEvent>(new EntityEventRefHandler<ToggleCellDrawComponent, ItemToggledEvent>(this.OnToggled));
    this.SubscribeLocalEvent<ToggleCellDrawComponent, PowerCellSlotEmptyEvent>(new EntityEventRefHandler<ToggleCellDrawComponent, PowerCellSlotEmptyEvent>(this.OnEmpty));
  }

  private void OnMapInit(Entity<ToggleCellDrawComponent> ent, ref MapInitEvent args)
  {
    this._cell.SetDrawEnabled((Entity<PowerCellDrawComponent>) ent.Owner, this._toggle.IsActivated((Entity<ItemToggleComponent>) ent.Owner));
  }

  private void OnActivateAttempt(
    Entity<ToggleCellDrawComponent> ent,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (this._cell.HasDrawCharge((EntityUid) ent, user: args.User) && this._cell.HasActivatableCharge((EntityUid) ent, user: args.User))
      return;
    args.Cancelled = true;
  }

  private void OnToggled(Entity<ToggleCellDrawComponent> ent, ref ItemToggledEvent args)
  {
    EntityUid owner = ent.Owner;
    PowerCellDrawComponent cellDrawComponent = this.Comp<PowerCellDrawComponent>(owner);
    this._cell.SetDrawEnabled((Entity<PowerCellDrawComponent>) (owner, cellDrawComponent), args.Activated);
  }

  private void OnEmpty(Entity<ToggleCellDrawComponent> ent, ref PowerCellSlotEmptyEvent args)
  {
    this._toggle.TryDeactivate((Entity<ItemToggleComponent>) ent.Owner);
  }
}
