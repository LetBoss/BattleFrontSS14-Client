// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mimicry.SharedMimicrySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Mimicry;

public sealed class SharedMimicrySystem : EntitySystem
{
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly TurfSystem _turf;
  [Dependency]
  private readonly ITileDefinitionManager _tiles;
  [Dependency]
  private readonly InventorySystem _inventory;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedDoAfterSystem _doAfter;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SharedHumanoidAppearanceSystem _humanoid;
  [Dependency]
  private readonly IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MimicryComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<MimicryComponent, BeingEquippedAttemptEvent>(this.OnEquipAttempt));
    this.SubscribeLocalEvent<MimicryComponent, GotEquippedEvent>(new EntityEventRefHandler<MimicryComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<MimicryComponent, GotUnequippedEvent>(new EntityEventRefHandler<MimicryComponent, GotUnequippedEvent>(this.OnUnequipped));
    this.SubscribeLocalEvent<MimicryComponent, MimicrySurfaceActionEvent>(new EntityEventRefHandler<MimicryComponent, MimicrySurfaceActionEvent>(this.OnAction));
    this.SubscribeLocalEvent<MimicryComponent, MimicryHoodToggleActionEvent>(new EntityEventRefHandler<MimicryComponent, MimicryHoodToggleActionEvent>(this.OnHoodToggle));
    this.SubscribeLocalEvent<MimicryComponent, MimicryDoAfterEvent>(new EntityEventRefHandler<MimicryComponent, MimicryDoAfterEvent>(this.OnDoAfter));
  }

  private void OnEquipAttempt(Entity<MimicryComponent> ent, ref BeingEquippedAttemptEvent args)
  {
    if (!ent.Comp.Hood.HasValue || !this._inventory.TryGetSlotEntity(args.EquipTarget, ent.Comp.HoodSlot, out EntityUid? _))
      return;
    args.Cancel();
    args.Reason = "mimicry-head-occupied";
    this._popup.PopupClient(this.Loc.GetString("mimicry-head-occupied"), args.EquipTarget, new EntityUid?(args.EquipTarget));
  }

  private void OnEquipped(Entity<MimicryComponent> ent, ref GotEquippedEvent args)
  {
    this._actions.AddAction(args.Equipee, ref ent.Comp.ActionEntity, (string) ent.Comp.Action, (EntityUid) ent);
    this._actions.AddAction(args.Equipee, ref ent.Comp.HoodToggleActionEntity, (string) ent.Comp.HoodToggleAction, (EntityUid) ent);
    if (this._net.IsServer)
    {
      EntProtoId? hood = ent.Comp.Hood;
      if (hood.HasValue)
      {
        EntProtoId valueOrDefault = hood.GetValueOrDefault();
        if (!ent.Comp.HoodUid.HasValue)
        {
          EntityUid itemUid = this.Spawn((string) valueOrDefault, this.Transform(args.Equipee).Coordinates);
          if (this._inventory.TryEquip(args.Equipee, itemUid, ent.Comp.HoodSlot, true, true))
            ent.Comp.HoodUid = new EntityUid?(itemUid);
          else
            this.QueueDel(new EntityUid?(itemUid));
        }
      }
    }
    this.SetHoodLayers(args.Equipee, ent.Comp, !ent.Comp.HoodDown);
  }

  private void OnUnequipped(Entity<MimicryComponent> ent, ref GotUnequippedEvent args)
  {
    SharedActionsSystem actions1 = this._actions;
    Entity<ActionsComponent> equipee1 = (Entity<ActionsComponent>) args.Equipee;
    EntityUid? nullable = ent.Comp.ActionEntity;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions1.RemoveAction(equipee1, action1);
    ent.Comp.ActionEntity = new EntityUid?();
    SharedActionsSystem actions2 = this._actions;
    Entity<ActionsComponent> equipee2 = (Entity<ActionsComponent>) args.Equipee;
    nullable = ent.Comp.HoodToggleActionEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions2.RemoveAction(equipee2, action2);
    ent.Comp.HoodToggleActionEntity = new EntityUid?();
    this.SetHoodLayers(args.Equipee, ent.Comp, false);
    if (!this._net.IsServer)
      return;
    nullable = ent.Comp.HoodUid;
    if (!nullable.HasValue)
      return;
    this.QueueDel(new EntityUid?(nullable.GetValueOrDefault()));
    ent.Comp.HoodUid = new EntityUid?();
  }

  private void OnHoodToggle(Entity<MimicryComponent> ent, ref MimicryHoodToggleActionEvent args)
  {
    if (args.Handled)
      return;
    ent.Comp.HoodDown = !ent.Comp.HoodDown;
    this.Dirty<MimicryComponent>(ent);
    this.SetHoodLayers(args.Performer, ent.Comp, !ent.Comp.HoodDown);
    this._popup.PopupClient(this.Loc.GetString(ent.Comp.HoodDown ? "mimicry-hood-down" : "mimicry-hood-up"), args.Performer, new EntityUid?(args.Performer));
    args.Handled = true;
  }

  private void SetHoodLayers(EntityUid wearer, MimicryComponent comp, bool hidden)
  {
    if (this._timing.ApplyingState || !this.HasComp<HumanoidAppearanceComponent>(wearer))
      return;
    foreach (HumanoidVisualLayers hoodHiddenLayer in comp.HoodHiddenLayers)
      this._humanoid.SetLayerVisibility((Entity<HumanoidAppearanceComponent>) wearer, hoodHiddenLayer, !hidden, new SlotFlags?(SlotFlags.HEAD));
  }

  private void OnAction(Entity<MimicryComponent> ent, ref MimicrySurfaceActionEvent args)
  {
    if (args.Handled)
      return;
    if (ent.Comp.MaxUses > 0 && ent.Comp.UsesDone >= ent.Comp.MaxUses)
    {
      this._popup.PopupClient(this.Loc.GetString("mimicry-used-up"), args.Performer, new EntityUid?(args.Performer));
    }
    else
    {
      TileRef? tile1;
      if (!this._turf.TryGetTileRef(args.Target, out tile1) || !(this._tiles[tile1.Value.Tile.TypeId] is ContentTileDefinition tile2) || !tile2.Sprite.HasValue)
        return;
      if (ent.Comp.SurfaceWhitelist.Count > 0 && !ent.Comp.SurfaceWhitelist.Contains((ProtoId<ContentTileDefinition>) tile2.ID))
      {
        this._popup.PopupClient(this.Loc.GetString("mimicry-bad-surface"), args.Performer, new EntityUid?(args.Performer));
      }
      else
      {
        MimicryDoAfterEvent mimicryDoAfterEvent = new MimicryDoAfterEvent(tile2.ID);
        EntityManager entityManager = this.EntityManager;
        EntityUid performer = args.Performer;
        TimeSpan delay = TimeSpan.FromSeconds((double) ent.Comp.MaskSeconds);
        MimicryDoAfterEvent @event = mimicryDoAfterEvent;
        EntityUid? eventTarget = new EntityUid?(ent.Owner);
        EntityUid? nullable = new EntityUid?(ent.Owner);
        EntityUid? target = new EntityUid?();
        EntityUid? used = nullable;
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, performer, delay, (DoAfterEvent) @event, eventTarget, target, used)
        {
          BreakOnMove = true,
          BreakOnDamage = true,
          NeedHand = false
        }))
          return;
        args.Handled = true;
      }
    }
  }

  private void OnDoAfter(Entity<MimicryComponent> ent, ref MimicryDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    ent.Comp.MimickedTile = (ProtoId<ContentTileDefinition>?) args.Tile;
    ++ent.Comp.UsesDone;
    this.Dirty<MimicryComponent>(ent);
    args.Handled = true;
  }
}
