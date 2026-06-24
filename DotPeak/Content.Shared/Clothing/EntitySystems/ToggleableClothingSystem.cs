// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.ToggleableClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Strip;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleableClothingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedStrippableSystem _strippable;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, ComponentInit>(new ComponentEventHandler<ToggleableClothingComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, MapInitEvent>(new ComponentEventHandler<ToggleableClothingComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, ToggleClothingEvent>(new ComponentEventHandler<ToggleableClothingComponent, ToggleClothingEvent>((object) this, __methodptr(OnToggleClothing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, GetItemActionsEvent>(new ComponentEventHandler<ToggleableClothingComponent, GetItemActionsEvent>((object) this, __methodptr(OnGetActions)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, ComponentRemove>(new ComponentEventHandler<ToggleableClothingComponent, ComponentRemove>((object) this, __methodptr(OnRemoveToggleable)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ToggleableClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnToggleableUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachedClothingComponent, InteractHandEvent>(new ComponentEventHandler<AttachedClothingComponent, InteractHandEvent>((object) this, __methodptr(OnInteractHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachedClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<AttachedClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnAttachedUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachedClothingComponent, ComponentRemove>(new ComponentEventHandler<AttachedClothingComponent, ComponentRemove>((object) this, __methodptr(OnRemoveAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachedClothingComponent, BeingUnequippedAttemptEvent>(new ComponentEventHandler<AttachedClothingComponent, BeingUnequippedAttemptEvent>((object) this, __methodptr(OnAttachedUnequipAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(new ComponentEventHandler<ToggleableClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>((object) this, __methodptr(GetRelayedVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, GetVerbsEvent<EquipmentVerb>>(new ComponentEventHandler<ToggleableClothingComponent, GetVerbsEvent<EquipmentVerb>>((object) this, __methodptr(OnGetVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachedClothingComponent, GetVerbsEvent<EquipmentVerb>>(new ComponentEventHandler<AttachedClothingComponent, GetVerbsEvent<EquipmentVerb>>((object) this, __methodptr(OnGetAttachedStripVerbsEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleableClothingComponent, ToggleClothingDoAfterEvent>(new ComponentEventHandler<ToggleableClothingComponent, ToggleClothingDoAfterEvent>((object) this, __methodptr(OnDoAfterComplete)), (Type[]) null, (Type[]) null);
  }

  private void GetRelayedVerbs(
    EntityUid uid,
    ToggleableClothingComponent component,
    InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
  {
    this.OnGetVerbs(uid, component, args.Args);
  }

  private void OnGetVerbs(
    EntityUid uid,
    ToggleableClothingComponent component,
    GetVerbsEvent<EquipmentVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !component.ClothingUid.HasValue || component.Container == null)
      return;
    string str = component.VerbText ?? (!component.ActionEntity.HasValue ? (string) null : this.Name(component.ActionEntity.Value, (MetaDataComponent) null));
    if (str == null || !this._inventorySystem.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), component.RequiredFlags))
      return;
    EntityUid parentUid = this.Transform(uid).ParentUid;
    if (EntityUid.op_Inequality(args.User, parentUid) && !component.StripDelay.HasValue)
      return;
    EquipmentVerb equipmentVerb1 = new EquipmentVerb();
    equipmentVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png"));
    equipmentVerb1.Text = this.Loc.GetString(str);
    EquipmentVerb equipmentVerb2 = equipmentVerb1;
    if (EntityUid.op_Equality(args.User, parentUid))
    {
      equipmentVerb2.EventTarget = uid;
      EquipmentVerb equipmentVerb3 = equipmentVerb2;
      ToggleClothingEvent toggleClothingEvent = new ToggleClothingEvent();
      toggleClothingEvent.Performer = args.User;
      equipmentVerb3.ExecutionEventArgs = (object) toggleClothingEvent;
    }
    else
      equipmentVerb2.Act = (Action) (() => this.StartDoAfter(args.User, uid, this.Transform(uid).ParentUid, component));
    args.Verbs.Add(equipmentVerb2);
  }

  private void StartDoAfter(
    EntityUid user,
    EntityUid item,
    EntityUid wearer,
    ToggleableClothingComponent component)
  {
    if (!component.StripDelay.HasValue)
      return;
    (TimeSpan timeSpan, bool Stealth) = this._strippable.GetStripTimeModifiers(user, wearer, new EntityUid?(item), component.StripDelay.Value);
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, timeSpan, (DoAfterEvent) new ToggleClothingDoAfterEvent(), new EntityUid?(item), new EntityUid?(wearer), new EntityUid?(item))
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      DistanceThreshold = new float?(2f)
    }) || Stealth)
      return;
    this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner-interact", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager)), (nameof (item), (object) item)), wearer, wearer, PopupType.Large);
  }

  private void OnGetAttachedStripVerbsEvent(
    EntityUid uid,
    AttachedClothingComponent component,
    GetVerbsEvent<EquipmentVerb> args)
  {
    this.OnGetVerbs(component.AttachedUid, this.Comp<ToggleableClothingComponent>(component.AttachedUid), args);
  }

  private void OnDoAfterComplete(
    EntityUid uid,
    ToggleableClothingComponent component,
    ToggleClothingDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    this.ToggleClothing(args.User, uid, component);
  }

  private void OnInteractHand(
    EntityUid uid,
    AttachedClothingComponent component,
    InteractHandEvent args)
  {
    ToggleableClothingComponent clothingComponent;
    if (args.Handled || !this.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref clothingComponent) || clothingComponent.Container == null || !this._inventorySystem.TryUnequip(this.Transform(uid).ParentUid, clothingComponent.Slot, force: true))
      return;
    this._containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer) clothingComponent.Container, (TransformComponent) null, false);
    args.Handled = true;
  }

  private void OnToggleableUnequip(
    EntityUid uid,
    ToggleableClothingComponent component,
    GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || component.Container == null || component.Container.ContainedEntity.HasValue || !component.ClothingUid.HasValue)
      return;
    this._inventorySystem.TryUnequip(args.Equipee, component.Slot, force: true, triggerHandContact: true);
  }

  private void OnRemoveToggleable(
    EntityUid uid,
    ToggleableClothingComponent component,
    ComponentRemove args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? actionEntity = component.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(action);
    if (!component.ClothingUid.HasValue || this._netMan.IsClient)
      return;
    this.QueueDel(new EntityUid?(component.ClothingUid.Value));
  }

  private void OnAttachedUnequipAttempt(
    EntityUid uid,
    AttachedClothingComponent component,
    BeingUnequippedAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnRemoveAttached(
    EntityUid uid,
    AttachedClothingComponent component,
    ComponentRemove args)
  {
    ToggleableClothingComponent clothingComponent;
    if (!this.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref clothingComponent) || clothingComponent.LifeStage > 6)
      return;
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? actionEntity = clothingComponent.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(action);
    this.RemComp(component.AttachedUid, (IComponent) clothingComponent);
  }

  private void OnAttachedUnequip(
    EntityUid uid,
    AttachedClothingComponent component,
    GotUnequippedEvent args)
  {
    ToggleableClothingComponent clothingComponent;
    if (this._timing.ApplyingState || component.LifeStage > 6 || !this.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref clothingComponent) || clothingComponent.LifeStage > 6 || !clothingComponent.ClothingUid.HasValue || clothingComponent.Container == null)
      return;
    this._containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(clothingComponent.ClothingUid.Value), (BaseContainer) clothingComponent.Container, (TransformComponent) null, false);
  }

  private void OnToggleClothing(
    EntityUid uid,
    ToggleableClothingComponent component,
    ToggleClothingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.ToggleClothing(args.Performer, uid, component);
  }

  private void ToggleClothing(
    EntityUid user,
    EntityUid target,
    ToggleableClothingComponent component)
  {
    if (component.Container == null || !component.ClothingUid.HasValue)
      return;
    EntityUid parentUid = this.Transform(target).ParentUid;
    if (!component.Container.ContainedEntity.HasValue)
    {
      this._inventorySystem.TryUnequip(user, parentUid, component.Slot, force: true);
    }
    else
    {
      EntityUid? entityUid;
      if (this._inventorySystem.TryGetSlotEntity(parentUid, component.Slot, out entityUid))
        this._popupSystem.PopupClient(this.Loc.GetString("toggleable-clothing-remove-first", ("entity", (object) entityUid)), user, new EntityUid?(user));
      else
        this._inventorySystem.TryEquip(user, parentUid, component.ClothingUid.Value, component.Slot, triggerHandContact: true);
    }
  }

  private void OnGetActions(
    EntityUid uid,
    ToggleableClothingComponent component,
    GetItemActionsEvent args)
  {
    if (!component.ClothingUid.HasValue || !component.ActionEntity.HasValue)
      return;
    SlotFlags? slotFlags = args.SlotFlags;
    SlotFlags requiredFlags1 = component.RequiredFlags;
    SlotFlags? nullable = slotFlags.HasValue ? new SlotFlags?(slotFlags.GetValueOrDefault() & requiredFlags1) : new SlotFlags?();
    SlotFlags requiredFlags2 = component.RequiredFlags;
    if (!(nullable.GetValueOrDefault() == requiredFlags2 & nullable.HasValue))
      return;
    args.AddAction(new EntityUid?(component.ActionEntity.Value));
  }

  private void OnInit(EntityUid uid, ToggleableClothingComponent component, ComponentInit args)
  {
    component.Container = this._containerSystem.EnsureContainer<ContainerSlot>(uid, component.ContainerId, (ContainerManagerComponent) null);
  }

  private void OnMapInit(EntityUid uid, ToggleableClothingComponent component, MapInitEvent args)
  {
    EntityUid? containedEntity = component.Container.ContainedEntity;
    if (containedEntity.HasValue)
    {
      containedEntity.GetValueOrDefault();
    }
    else
    {
      if (!component.ClothingUid.HasValue || !component.ActionEntity.HasValue)
      {
        TransformComponent transformComponent = this.Transform(uid);
        component.ClothingUid = new EntityUid?(this.Spawn(EntProtoId.op_Implicit(component.ClothingPrototype), transformComponent.Coordinates));
        AttachedClothingComponent clothingComponent = this.EnsureComp<AttachedClothingComponent>(component.ClothingUid.Value);
        clothingComponent.AttachedUid = uid;
        this.Dirty(component.ClothingUid.Value, (IComponent) clothingComponent, (MetaDataComponent) null);
        this._containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(component.ClothingUid.Value), (BaseContainer) component.Container, transformComponent, false);
        this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      }
      ActionComponent action;
      if (!this._actionContainer.EnsureAction(uid, ref component.ActionEntity, out action, EntProtoId.op_Implicit(component.Action)))
        return;
      this._actionsSystem.SetEntityIcon(Entity<ActionComponent>.op_Implicit((component.ActionEntity.Value, action)), component.ClothingUid);
    }
  }
}
