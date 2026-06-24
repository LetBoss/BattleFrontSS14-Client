// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.FarSight.FarSightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.FarSight;

public sealed class FarSightSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContentEyeSystem _eye;
  [Dependency]
  private InventorySystem _inventory;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<FarSightItemComponent, GetItemActionsEvent>(new EntityEventRefHandler<FarSightItemComponent, GetItemActionsEvent>(this.OnFarSightGetItemActions));
    this.SubscribeLocalEvent<FarSightItemComponent, FarSightActionEvent>(new EntityEventRefHandler<FarSightItemComponent, FarSightActionEvent>(this.OnFarSightAction));
    this.SubscribeLocalEvent<FarSightItemComponent, GotUnequippedEvent>(new EntityEventRefHandler<FarSightItemComponent, GotUnequippedEvent>(this.OnFarSightUnequipped));
    this.SubscribeLocalEvent<FarSightItemComponent, GotEquippedEvent>(new EntityEventRefHandler<FarSightItemComponent, GotEquippedEvent>(this.OnFarSightEquipped));
  }

  private void OnFarSightGetItemActions(
    Entity<FarSightItemComponent> ent,
    ref GetItemActionsEvent args)
  {
    if (args.InHands || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), ent.Comp.Slots))
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty<FarSightItemComponent>(ent);
  }

  private void OnFarSightAction(Entity<FarSightItemComponent> ent, ref FarSightActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    ent.Comp.Enabled = !ent.Comp.Enabled;
    this.Dirty<FarSightItemComponent>(ent);
    EntityUid performer = args.Performer;
    this.SetZoom(ent.Comp.Enabled, performer, ent.Comp);
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = ent.Comp.Enabled ? 1 : 0;
    actions.SetToggled(action2, num != 0);
    this._appearance.SetData((EntityUid) ent, (Enum) FarSightItemVisuals.Active, (object) ent.Comp.Enabled);
  }

  private void OnFarSightEquipped(Entity<FarSightItemComponent> ent, ref GotEquippedEvent args)
  {
    EntityUid equipee = args.Equipee;
    if (!this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), ent.Comp.Slots))
      return;
    this.SetZoom(ent.Comp.Enabled, equipee, ent.Comp);
  }

  private void OnFarSightUnequipped(Entity<FarSightItemComponent> ent, ref GotUnequippedEvent args)
  {
    EntityUid equipee = args.Equipee;
    if (this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), ent.Comp.Slots))
      return;
    this.SetZoom(false, equipee, ent.Comp);
  }

  private void SetZoom(bool activated, EntityUid user, FarSightItemComponent comp)
  {
    if (activated)
    {
      this._eye.SetMaxZoom(user, comp.Zoom);
      this._eye.SetZoom(user, comp.Zoom);
    }
    else
    {
      EyeComponent comp1;
      if (this.TryComp<EyeComponent>(user, out comp1))
        this._eye.SetMaxZoom(user, comp1.Zoom);
      this._eye.ResetZoom(user);
    }
  }
}
