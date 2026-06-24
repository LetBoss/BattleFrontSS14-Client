// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointSlotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class HardpointSlotSystem : EntitySystem
{
  [Dependency]
  private readonly SharedDoAfterSystem _doAfter;
  [Dependency]
  private readonly HardpointSystem _hardpoints;
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SharedToolSystem _tool;
  [Dependency]
  private readonly SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HardpointSlotsComponent, ItemSlotInsertAttemptEvent>(new EntityEventRefHandler<HardpointSlotsComponent, ItemSlotInsertAttemptEvent>(this.OnInsertAttempt));
    this.SubscribeLocalEvent<HardpointSlotsComponent, HardpointInsertDoAfterEvent>(new EntityEventRefHandler<HardpointSlotsComponent, HardpointInsertDoAfterEvent>(this.OnInsertDoAfter));
    this.SubscribeLocalEvent<HardpointSlotsComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<HardpointSlotsComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetRemoveVerbs));
    this.SubscribeLocalEvent<HardpointSlotsComponent, InteractUsingEvent>(new EntityEventRefHandler<HardpointSlotsComponent, InteractUsingEvent>(this.OnSlotsInteractUsing), new Type[1]
    {
      typeof (ItemSlotsSystem)
    });
    this.SubscribeLocalEvent<HardpointSlotsComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<HardpointSlotsComponent, BoundUIOpenedEvent>(this.OnHardpointUiOpened));
    this.SubscribeLocalEvent<HardpointSlotsComponent, BoundUIClosedEvent>(new EntityEventRefHandler<HardpointSlotsComponent, BoundUIClosedEvent>(this.OnHardpointUiClosed));
    this.SubscribeLocalEvent<HardpointSlotsComponent, HardpointRemoveMessage>(new EntityEventRefHandler<HardpointSlotsComponent, HardpointRemoveMessage>(this.OnHardpointRemoveMessage));
    this.SubscribeLocalEvent<HardpointSlotsComponent, HardpointRemoveDoAfterEvent>(new EntityEventRefHandler<HardpointSlotsComponent, HardpointRemoveDoAfterEvent>(this.OnHardpointRemoveDoAfter));
  }

  public void DisableEjectForAllSlots(Entity<HardpointSlotsComponent> ent)
  {
    ItemSlotsComponent comp;
    if (!this.TryComp<ItemSlotsComponent>(ent.Owner, out comp))
      return;
    foreach (HardpointSlot slot in ent.Comp.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(ent.Owner, slot.Id, out itemSlot, comp))
        this._itemSlots.SetEjectFlags(ent.Owner, itemSlot, true, itemSlot.EjectOnInteract, itemSlot.EjectOnUse, comp);
    }
  }

  private void OnInsertAttempt(
    Entity<HardpointSlotsComponent> ent,
    ref ItemSlotInsertAttemptEvent args)
  {
    HardpointSlot slot;
    if (!args.User.HasValue || !this._hardpoints.TryGetSlot(ent.Comp, args.Slot.ID, out slot) || ent.Comp.CompletingInserts.Contains(slot.Id))
      return;
    if (!this._hardpoints.IsValidHardpoint(args.Item, ent.Comp, slot))
    {
      args.Cancelled = true;
    }
    else
    {
      if ((double) slot.InsertDelay <= 0.0)
        return;
      HashSet<EntityUid> pendingInsertUsers1 = ent.Comp.PendingInsertUsers;
      EntityUid? user1 = args.User;
      EntityUid entityUid1 = user1.Value;
      if (pendingInsertUsers1.Contains(entityUid1))
        args.Cancelled = true;
      else if (!ent.Comp.PendingInserts.Add(slot.Id))
      {
        args.Cancelled = true;
      }
      else
      {
        args.Cancelled = true;
        HashSet<EntityUid> pendingInsertUsers2 = ent.Comp.PendingInsertUsers;
        user1 = args.User;
        EntityUid entityUid2 = user1.Value;
        pendingInsertUsers2.Add(entityUid2);
        EntityManager entityManager = this.EntityManager;
        user1 = args.User;
        EntityUid user2 = user1.Value;
        double insertDelay = (double) slot.InsertDelay;
        HardpointInsertDoAfterEvent @event = new HardpointInsertDoAfterEvent(slot.Id);
        EntityUid? eventTarget = new EntityUid?(ent.Owner);
        EntityUid? target = new EntityUid?(ent.Owner);
        EntityUid? used = new EntityUid?(args.Item);
        if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user2, (float) insertDelay, (DoAfterEvent) @event, eventTarget, target, used)
        {
          BreakOnMove = true,
          BreakOnDamage = true,
          BreakOnHandChange = true,
          BreakOnDropItem = true,
          BreakOnWeightlessMove = true,
          NeedHand = true,
          RequireCanInteract = true,
          DuplicateCondition = DuplicateConditions.SameEvent
        }))
          return;
        ent.Comp.PendingInserts.Remove(slot.Id);
        HashSet<EntityUid> pendingInsertUsers3 = ent.Comp.PendingInsertUsers;
        user1 = args.User;
        EntityUid entityUid3 = user1.Value;
        pendingInsertUsers3.Remove(entityUid3);
      }
    }
  }

  private void OnInsertDoAfter(
    Entity<HardpointSlotsComponent> ent,
    ref HardpointInsertDoAfterEvent args)
  {
    ent.Comp.PendingInserts.Remove(args.SlotId);
    ent.Comp.PendingInsertUsers.Remove(args.User);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    ItemSlotsComponent comp;
    HardpointSlot slot;
    ItemSlot itemSlot;
    if (string.IsNullOrEmpty(args.SlotId) || !this.TryComp<ItemSlotsComponent>(ent.Owner, out comp) || !this._hardpoints.TryGetSlot(ent.Comp, args.SlotId, out slot) || !this._itemSlots.TryGetSlot(ent.Owner, args.SlotId, out itemSlot, comp) || !this._hardpoints.IsValidHardpoint(valueOrDefault, ent.Comp, slot))
      return;
    ent.Comp.CompletingInserts.Add(args.SlotId);
    this._itemSlots.TryInsertFromHand(ent.Owner, itemSlot, args.User);
    ent.Comp.CompletingInserts.Remove(args.SlotId);
  }

  private void OnGetRemoveVerbs(
    Entity<HardpointSlotsComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    ItemSlotsComponent comp;
    if (!args.CanAccess || !args.CanInteract || !this.TryComp<ItemSlotsComponent>(ent.Owner, out comp))
      return;
    foreach (HardpointSlot slot in ent.Comp.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(ent.Owner, slot.Id, out itemSlot, comp) && itemSlot.HasItem)
      {
        EntityUid? nullable = itemSlot.Item;
        if (!this.HasComp<HardpointNoRemoveComponent>(nullable.Value))
        {
          EntityUid user = args.User;
          string slotId = slot.Id;
          InteractionVerb interactionVerb1 = new InteractionVerb();
          interactionVerb1.Act = (Action) (() => this.TryStartHardpointRemoval(ent.Owner, ent.Comp, user, slotId));
          interactionVerb1.Category = VerbCategory.Eject;
          ILocalizationManager loc = this.Loc;
          nullable = itemSlot.Item;
          (string, object) valueTuple = ("slot", (object) this.Name(nullable.Value));
          interactionVerb1.Text = loc.GetString("rmc-hardpoint-remove-verb", valueTuple);
          interactionVerb1.Priority = itemSlot.Priority;
          interactionVerb1.IconEntity = this.GetNetEntity(itemSlot.Item);
          InteractionVerb interactionVerb2 = interactionVerb1;
          args.Verbs.Add(interactionVerb2);
        }
      }
    }
    this.AddTurretRemoveVerbs(ent, ref args, comp);
  }

  private void OnSlotsInteractUsing(
    Entity<HardpointSlotsComponent> ent,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    if (this.TryInsertTurretAttachment(ent, args.User, args.Used))
    {
      args.Handled = true;
    }
    else
    {
      if (!this._tool.HasQuality(args.Used, (string) ent.Comp.RemoveToolQuality) || !this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) HardpointUiKey.Key, args.User))
        return;
      this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
      args.Handled = true;
    }
  }

  private void OnHardpointUiOpened(Entity<HardpointSlotsComponent> ent, ref BoundUIOpenedEvent args)
  {
    if (!object.Equals((object) args.UiKey, (object) HardpointUiKey.Key))
      return;
    ent.Comp.LastUiError = (string) null;
    this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
  }

  private void OnHardpointUiClosed(Entity<HardpointSlotsComponent> ent, ref BoundUIClosedEvent args)
  {
    if (!object.Equals((object) args.UiKey, (object) HardpointUiKey.Key))
      return;
    ent.Comp.PendingRemovals.Clear();
    ent.Comp.LastUiError = (string) null;
  }

  private void OnHardpointRemoveMessage(
    Entity<HardpointSlotsComponent> ent,
    ref HardpointRemoveMessage args)
  {
    if (!object.Equals((object) args.UiKey, (object) HardpointUiKey.Key) || args.Actor == new EntityUid() || !this.Exists(args.Actor))
      return;
    this.TryStartHardpointRemoval(ent.Owner, ent.Comp, args.Actor, args.SlotId);
  }

  private void OnHardpointRemoveDoAfter(
    Entity<HardpointSlotsComponent> ent,
    ref HardpointRemoveDoAfterEvent args)
  {
    ent.Comp.PendingRemovals.Remove(args.SlotId);
    if (args.Cancelled || args.Handled)
    {
      if (args.Cancelled)
      {
        ent.Comp.LastUiError = "Hardpoint removal cancelled.";
        this._hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
      }
      this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
      this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
    }
    else
    {
      args.Handled = true;
      ItemSlotsComponent comp;
      if (!this.TryComp<ItemSlotsComponent>(ent.Owner, out comp))
      {
        ent.Comp.LastUiError = "Unable to access hardpoint slots.";
        this._hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
        this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
        this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
      }
      else if (!this._hardpoints.TryGetSlot(ent.Comp, args.SlotId, out HardpointSlot _))
      {
        ent.Comp.LastUiError = "That hardpoint slot is no longer available.";
        this._hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
        this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, comp);
        this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
      }
      else
      {
        ItemSlot itemSlot;
        if (!this._itemSlots.TryGetSlot(ent.Owner, args.SlotId, out itemSlot, comp) || !itemSlot.HasItem)
        {
          ent.Comp.LastUiError = "No hardpoint is installed in that slot.";
          this._hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
          this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, comp);
          this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
        }
        else if (!this._itemSlots.TryEjectToHands(ent.Owner, itemSlot, new EntityUid?(args.User), true))
        {
          ent.Comp.LastUiError = "Couldn't remove the hardpoint. Free a hand and try again.";
          this._hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
          this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, comp);
          this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
        }
        else
        {
          ent.Comp.LastUiError = (string) null;
          this._hardpoints.SetContainingVehicleUiError(ent.Owner, (string) null);
          this._hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, comp);
          this._hardpoints.UpdateContainingVehicleUi(ent.Owner);
          this._hardpoints.RefreshCanRun(ent.Owner);
        }
      }
    }
  }

  private void TryStartHardpointRemoval(
    EntityUid uid,
    HardpointSlotsComponent component,
    EntityUid user,
    string? slotId,
    EntityUid? uiOwnerUid = null,
    HardpointSlotsComponent? uiOwnerComp = null)
  {
    int num = !uiOwnerUid.HasValue ? 1 : (uiOwnerComp == null ? 1 : 0);
    uiOwnerUid.GetValueOrDefault();
    if (!uiOwnerUid.HasValue)
      uiOwnerUid = new EntityUid?(uid);
    if (uiOwnerComp == null)
      uiOwnerComp = component;
    if (num != 0)
      uiOwnerComp.LastUiError = (string) null;
    if (string.IsNullOrWhiteSpace(slotId))
    {
      SetError("Invalid hardpoint slot.");
      RefreshUi();
    }
    else
    {
      string parentSlotId;
      string childSlotId;
      if (VehicleTurretSlotIds.TryParse(slotId, out parentSlotId, out childSlotId))
      {
        ItemSlotsComponent comp1;
        if (!this.TryComp<ItemSlotsComponent>(uid, out comp1) || !this._hardpoints.TryGetSlot(component, parentSlotId, out HardpointSlot _))
        {
          SetError("Unable to find that turret slot.");
          RefreshUi(comp1);
        }
        else
        {
          ItemSlot itemSlot;
          if (!this._itemSlots.TryGetSlot(uid, parentSlotId, out itemSlot, comp1) || !itemSlot.HasItem)
          {
            SetError("Install a turret before removing turret hardpoints.");
            RefreshUi(comp1);
          }
          else
          {
            EntityUid uid1 = itemSlot.Item.Value;
            HardpointSlotsComponent comp2;
            if (!this.TryComp<HardpointSlotsComponent>(uid1, out comp2))
            {
              SetError("Turret hardpoint slots are unavailable.");
              RefreshUi(comp1);
            }
            else
            {
              this.TryStartHardpointRemoval(uid1, comp2, user, childSlotId, uiOwnerUid, uiOwnerComp);
              RefreshUi(comp1);
            }
          }
        }
      }
      else
      {
        ItemSlotsComponent comp3;
        if (!this.TryComp<ItemSlotsComponent>(uid, out comp3))
        {
          SetError("Unable to access hardpoint slots.");
          RefreshUi();
        }
        else
        {
          HardpointSlot slot;
          if (!this._hardpoints.TryGetSlot(component, slotId, out slot))
          {
            SetError("That hardpoint slot does not exist.");
            RefreshUi(comp3);
          }
          else
          {
            ItemSlot itemSlot;
            if (!this._itemSlots.TryGetSlot(uid, slotId, out itemSlot, comp3) || !itemSlot.HasItem)
            {
              SetError("No hardpoint is installed in that slot.");
              RefreshUi(comp3);
            }
            else
            {
              HardpointSlotsComponent comp4;
              ItemSlotsComponent comp5;
              EntityUid? nullable;
              if (this.TryComp<HardpointSlotsComponent>(itemSlot.Item.Value, out comp4) && this.TryComp<ItemSlotsComponent>(itemSlot.Item.Value, out comp5))
              {
                HardpointSystem hardpoints = this._hardpoints;
                nullable = itemSlot.Item;
                EntityUid owner = nullable.Value;
                HardpointSlotsComponent slots = comp4;
                ItemSlotsComponent itemSlots = comp5;
                if (hardpoints.HasAttachedHardpoints(owner, slots, itemSlots))
                {
                  this._popup.PopupEntity("Remove the turret attachments before removing the turret.", uid, user);
                  SetError("Remove the turret attachments before removing the turret.");
                  RefreshUi(comp3);
                  return;
                }
              }
              nullable = itemSlot.Item;
              if (this.HasComp<HardpointNoRemoveComponent>(nullable.Value))
              {
                string str = this.Loc.GetString("rmc-hardpoint-remove-blocked");
                this._popup.PopupEntity(str, uid, user);
                SetError(str);
                RefreshUi(comp3);
              }
              else if (component.PendingInserts.Contains(slotId) || component.CompletingInserts.Contains(slotId))
              {
                this._popup.PopupEntity("Finish installing that hardpoint before removing it.", user, user);
                SetError("Finish installing that hardpoint before removing it.");
                RefreshUi(comp3);
              }
              else
              {
                EntityUid tool;
                if (!this._hardpoints.TryGetPryingTool(user, component.RemoveToolQuality, out tool))
                {
                  string str = this.Loc.GetString("rmc-hardpoint-remove-need-tool");
                  this._popup.PopupEntity(str, user, user);
                  SetError(str);
                  RefreshUi(comp3);
                }
                else if (!component.PendingRemovals.Add(slotId))
                {
                  SetError("That hardpoint is already being removed.");
                  RefreshUi(comp3);
                }
                else
                {
                  float seconds = (double) slot.RemoveDelay > 0.0 ? slot.RemoveDelay : slot.InsertDelay;
                  if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, seconds, (DoAfterEvent) new HardpointRemoveDoAfterEvent(slotId), new EntityUid?(uid), new EntityUid?(uid), new EntityUid?(tool))
                  {
                    BreakOnMove = true,
                    BreakOnDamage = true,
                    BreakOnHandChange = true,
                    BreakOnDropItem = true,
                    BreakOnWeightlessMove = true,
                    NeedHand = true,
                    RequireCanInteract = true,
                    DuplicateCondition = DuplicateConditions.SameEvent
                  }))
                  {
                    component.PendingRemovals.Remove(slotId);
                    SetError("Couldn't start hardpoint removal.");
                    RefreshUi(comp3);
                  }
                  else
                  {
                    uiOwnerComp.LastUiError = (string) null;
                    RefreshUi(comp3);
                  }
                }
              }
            }
          }
        }
      }
    }

    void RefreshUi(ItemSlotsComponent? currentItemSlots = null)
    {
      this._hardpoints.UpdateHardpointUi(uid, component, currentItemSlots);
      if (!(uiOwnerUid.Value != uid) && uiOwnerComp == component)
        return;
      this._hardpoints.UpdateHardpointUi(uiOwnerUid.Value, uiOwnerComp);
    }

    void SetError(string error) => uiOwnerComp.LastUiError = error;
  }

  private bool TryInsertTurretAttachment(
    Entity<HardpointSlotsComponent> ent,
    EntityUid user,
    EntityUid used)
  {
    ItemSlotsComponent comp1;
    if (!this.HasComp<HardpointItemComponent>(used) || !this.TryComp<ItemSlotsComponent>(ent.Owner, out comp1))
      return false;
    bool flag1 = this.HasComp<VehicleTurretAttachmentComponent>(used);
    bool flag2 = false;
    foreach (HardpointSlot slot in ent.Comp.Slots)
    {
      ItemSlot itemSlot;
      if (this._hardpoints.IsValidHardpoint(used, ent.Comp, slot) && this._itemSlots.TryGetSlot(ent.Owner, slot.Id, out itemSlot, comp1) && !itemSlot.HasItem)
      {
        flag2 = true;
        break;
      }
    }
    if (!flag1 & flag2)
      return false;
    foreach (HardpointSlot slot1 in ent.Comp.Slots)
    {
      ItemSlot itemSlot1;
      if (this._itemSlots.TryGetSlot(ent.Owner, slot1.Id, out itemSlot1, comp1) && itemSlot1.HasItem)
      {
        EntityUid uid = itemSlot1.Item.Value;
        HardpointSlotsComponent comp2;
        ItemSlotsComponent comp3;
        if (this.TryComp<HardpointSlotsComponent>(uid, out comp2) && this.TryComp<ItemSlotsComponent>(uid, out comp3))
        {
          foreach (HardpointSlot slot2 in comp2.Slots)
          {
            ItemSlot itemSlot2;
            if (this._hardpoints.IsValidHardpoint(used, comp2, slot2) && this._itemSlots.TryGetSlot(uid, slot2.Id, out itemSlot2, comp3) && !itemSlot2.HasItem)
            {
              this._itemSlots.TryInsertFromHand(uid, itemSlot2, user);
              return true;
            }
          }
        }
      }
    }
    if (!flag1)
      return false;
    this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-turret-no-base"), ent.Owner, new EntityUid?(user));
    return true;
  }

  private void AddTurretRemoveVerbs(
    Entity<HardpointSlotsComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args,
    ItemSlotsComponent itemSlots)
  {
    foreach (HardpointSlot slot1 in ent.Comp.Slots)
    {
      ItemSlot itemSlot1;
      if (this._itemSlots.TryGetSlot(ent.Owner, slot1.Id, out itemSlot1, itemSlots) && itemSlot1.HasItem)
      {
        EntityUid? nullable = itemSlot1.Item;
        EntityUid turretUid = nullable.Value;
        ItemSlotsComponent comp;
        HardpointSlotsComponent turretSlots;
        if (this.TryComp<HardpointSlotsComponent>(turretUid, out turretSlots) && this.TryComp<ItemSlotsComponent>(turretUid, out comp))
        {
          foreach (HardpointSlot slot2 in turretSlots.Slots)
          {
            ItemSlot itemSlot2;
            if (this._itemSlots.TryGetSlot(turretUid, slot2.Id, out itemSlot2, comp) && itemSlot2.HasItem)
            {
              nullable = itemSlot2.Item;
              if (!this.HasComp<HardpointNoRemoveComponent>(nullable.Value))
              {
                EntityUid user = args.User;
                string slotId = slot2.Id;
                InteractionVerb interactionVerb1 = new InteractionVerb();
                interactionVerb1.Act = (Action) (() => this.TryStartHardpointRemoval(turretUid, turretSlots, user, slotId));
                interactionVerb1.Category = VerbCategory.Eject;
                ILocalizationManager loc = this.Loc;
                nullable = itemSlot2.Item;
                (string, object) valueTuple = ("slot", (object) this.Name(nullable.Value));
                interactionVerb1.Text = loc.GetString("rmc-hardpoint-remove-verb", valueTuple);
                interactionVerb1.Priority = itemSlot2.Priority;
                interactionVerb1.IconEntity = this.GetNetEntity(itemSlot2.Item);
                InteractionVerb interactionVerb2 = interactionVerb1;
                args.Verbs.Add(interactionVerb2);
              }
            }
          }
        }
      }
    }
  }
}
