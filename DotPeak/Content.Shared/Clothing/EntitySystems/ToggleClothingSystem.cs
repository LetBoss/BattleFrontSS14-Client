// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.ToggleClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Toggleable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleClothingSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ItemToggleSystem _toggle;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleClothingComponent, MapInitEvent>(new EntityEventRefHandler<ToggleClothingComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleClothingComponent, GetItemActionsEvent>(new EntityEventRefHandler<ToggleClothingComponent, GetItemActionsEvent>((object) this, __methodptr(OnGetActions)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleClothingComponent, ToggleActionEvent>(new EntityEventRefHandler<ToggleClothingComponent, ToggleActionEvent>((object) this, __methodptr(OnToggleAction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleClothingComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<ToggleClothingComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnUnequipped)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<ToggleClothingComponent> ent, ref MapInitEvent args)
  {
    EntityUid entityUid;
    ToggleClothingComponent clothingComponent1;
    ent.Deconstruct(ref entityUid, ref clothingComponent1);
    EntityUid performer = entityUid;
    ToggleClothingComponent clothingComponent2 = clothingComponent1;
    if (string.IsNullOrEmpty(EntProtoId<InstantActionComponent>.op_Implicit(clothingComponent2.Action)))
      return;
    this._actions.AddAction(performer, ref clothingComponent2.ActionEntity, EntProtoId<InstantActionComponent>.op_Implicit(clothingComponent2.Action), new EntityUid());
    SharedActionsSystem actions = this._actions;
    EntityUid? actionEntity = clothingComponent2.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    int num = this._toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) ? 1 : 0;
    actions.SetToggled(action, num != 0);
    this.Dirty(performer, (IComponent) clothingComponent2, (MetaDataComponent) null);
  }

  private void OnGetActions(Entity<ToggleClothingComponent> ent, ref GetItemActionsEvent args)
  {
    if (args.InHands && ent.Comp.MustEquip)
      return;
    ToggleClothingCheckEvent clothingCheckEvent = new ToggleClothingCheckEvent(args.User);
    this.RaiseLocalEvent<ToggleClothingCheckEvent>(Entity<ToggleClothingComponent>.op_Implicit(ent), ref clothingCheckEvent, false);
    if (clothingCheckEvent.Cancelled)
      return;
    args.AddAction(ent.Comp.ActionEntity);
  }

  private void OnToggleAction(Entity<ToggleClothingComponent> ent, ref ToggleActionEvent args)
  {
    args.Handled = this._toggle.Toggle(Entity<ItemToggleComponent>.op_Implicit(ent.Owner), new EntityUid?(args.Performer));
  }

  private void OnUnequipped(
    Entity<ToggleClothingComponent> ent,
    ref ClothingGotUnequippedEvent args)
  {
    if (!ent.Comp.DisableOnUnequip)
      return;
    this._toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(ent.Owner), new EntityUid?(args.Wearer));
  }
}
