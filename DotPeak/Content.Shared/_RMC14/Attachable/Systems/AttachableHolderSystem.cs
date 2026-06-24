// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableHolderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Item;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Containers;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableHolderSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedVerbSystem _verbSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableHolderComponent, AttachableAttachDoAfterEvent>(new ComponentEventHandler<AttachableHolderComponent, AttachableAttachDoAfterEvent>(this.OnAttachDoAfter));
    this.SubscribeLocalEvent<AttachableHolderComponent, AttachableDetachDoAfterEvent>(new ComponentEventHandler<AttachableHolderComponent, AttachableDetachDoAfterEvent>(this.OnDetachDoAfter));
    this.SubscribeLocalEvent<AttachableHolderComponent, AttachableHolderAttachToSlotMessage>(new ComponentEventHandler<AttachableHolderComponent, AttachableHolderAttachToSlotMessage>(this.OnAttachableHolderAttachToSlotMessage));
    this.SubscribeLocalEvent<AttachableHolderComponent, AttachableHolderDetachMessage>(new ComponentEventHandler<AttachableHolderComponent, AttachableHolderDetachMessage>(this.OnAttachableHolderDetachMessage));
    this.SubscribeLocalEvent<AttachableHolderComponent, GunShotEvent>(new EntityEventRefHandler<AttachableHolderComponent, GunShotEvent>(this.RelayEvent<GunShotEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, BoundUIOpenedEvent>(new ComponentEventHandler<AttachableHolderComponent, BoundUIOpenedEvent>(this.OnAttachableHolderUiOpened));
    this.SubscribeLocalEvent<AttachableHolderComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<AttachableHolderComponent, EntInsertedIntoContainerMessage>(this.OnAttached));
    this.SubscribeLocalEvent<AttachableHolderComponent, MapInitEvent>(new EntityEventRefHandler<AttachableHolderComponent, MapInitEvent>(this.OnHolderMapInit), after: new Type[1]
    {
      typeof (ContainerFillSystem)
    });
    this.SubscribeLocalEvent<AttachableHolderComponent, GetVerbsEvent<EquipmentVerb>>(new EntityEventRefHandler<AttachableHolderComponent, GetVerbsEvent<EquipmentVerb>>(this.OnAttachableHolderGetVerbs));
    this.SubscribeLocalEvent<AttachableHolderComponent, GotEquippedHandEvent>(new EntityEventRefHandler<AttachableHolderComponent, GotEquippedHandEvent>(this.RelayEvent<GotEquippedHandEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<AttachableHolderComponent, GotUnequippedHandEvent>(this.RelayEvent<GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<AttachableHolderComponent, GunRefreshModifiersEvent>(this.RelayEvent<GunRefreshModifiersEvent>), after: new Type[1]
    {
      typeof (SharedWieldableSystem)
    });
    this.SubscribeLocalEvent<AttachableHolderComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<AttachableHolderComponent, BeforeRangedInteractEvent>(this.OnAttachableHolderBeforeRangedInteract), new Type[1]
    {
      typeof (SharedStorageSystem)
    });
    this.SubscribeLocalEvent<AttachableHolderComponent, InteractUsingEvent>(new EntityEventRefHandler<AttachableHolderComponent, InteractUsingEvent>(this.OnAttachableHolderInteractUsing));
    this.SubscribeLocalEvent<AttachableHolderComponent, AfterInteractEvent>(new EntityEventRefHandler<AttachableHolderComponent, AfterInteractEvent>(this.OnAttachableHolderAfterInteract));
    this.SubscribeLocalEvent<AttachableHolderComponent, ActivateInWorldEvent>(new EntityEventRefHandler<AttachableHolderComponent, ActivateInWorldEvent>(this.OnAttachableHolderInteractInWorld), new Type[1]
    {
      typeof (CMGunSystem)
    });
    this.SubscribeLocalEvent<AttachableHolderComponent, ItemWieldedEvent>(new EntityEventRefHandler<AttachableHolderComponent, ItemWieldedEvent>(this.OnHolderWielded));
    this.SubscribeLocalEvent<AttachableHolderComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<AttachableHolderComponent, ItemUnwieldedEvent>(this.OnHolderUnwielded));
    this.SubscribeLocalEvent<AttachableHolderComponent, UniqueActionEvent>(new EntityEventRefHandler<AttachableHolderComponent, UniqueActionEvent>(this.OnAttachableHolderUniqueAction));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetGunDamageModifierEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetGunDamageModifierEvent>(this.RelayEvent<GetGunDamageModifierEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GunMuzzleFlashAttemptEvent>(new EntityEventRefHandler<AttachableHolderComponent, GunMuzzleFlashAttemptEvent>(this.RelayEvent<GunMuzzleFlashAttemptEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, HandDeselectedEvent>(new EntityEventRefHandler<AttachableHolderComponent, HandDeselectedEvent>(this.RelayEvent<HandDeselectedEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, MeleeHitEvent>(new EntityEventRefHandler<AttachableHolderComponent, MeleeHitEvent>(this.RelayEvent<MeleeHitEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetWieldableSpeedModifiersEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetWieldableSpeedModifiersEvent>(this.RelayEvent<GetWieldableSpeedModifiersEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetWieldDelayEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetWieldDelayEvent>(this.RelayEvent<GetWieldDelayEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, ContainerGettingInsertedAttemptEvent>(new EntityEventRefHandler<AttachableHolderComponent, ContainerGettingInsertedAttemptEvent>(this.RelayEvent<ContainerGettingInsertedAttemptEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, ContainerGettingRemovedAttemptEvent>(new EntityEventRefHandler<AttachableHolderComponent, ContainerGettingRemovedAttemptEvent>(this.RelayEvent<ContainerGettingRemovedAttemptEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<AttachableHolderComponent, EntGotRemovedFromContainerMessage>(this.RelayEvent<EntGotRemovedFromContainerMessage>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetItemSizeModifiersEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetItemSizeModifiersEvent>(this.RelayEvent<GetItemSizeModifiersEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetFireModeValuesEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetFireModeValuesEvent>(this.RelayEvent<GetFireModeValuesEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetFireModesEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetFireModesEvent>(this.RelayEvent<GetFireModesEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetDamageFalloffEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetDamageFalloffEvent>(this.RelayEvent<GetDamageFalloffEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GetWeaponAccuracyEvent>(new EntityEventRefHandler<AttachableHolderComponent, GetWeaponAccuracyEvent>(this.RelayEvent<GetWeaponAccuracyEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, GunGetAmmoSpreadEvent>(new EntityEventRefHandler<AttachableHolderComponent, GunGetAmmoSpreadEvent>(this.RelayEvent<GunGetAmmoSpreadEvent>));
    this.SubscribeLocalEvent<AttachableHolderComponent, DroppedEvent>(new EntityEventRefHandler<AttachableHolderComponent, DroppedEvent>(this.RelayEvent<DroppedEvent>));
    CommandBinds.Builder.Bind(CMKeyFunctions.RMCActivateAttachableBarrel, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.ToggleAttachable(attachedEntity.GetValueOrDefault(), "rmc-aslot-barrel");
    }), handle: false)).Bind(CMKeyFunctions.RMCActivateAttachableRail, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.ToggleAttachable(attachedEntity.GetValueOrDefault(), "rmc-aslot-rail");
    }), handle: false)).Bind(CMKeyFunctions.RMCActivateAttachableStock, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.ToggleAttachable(attachedEntity.GetValueOrDefault(), "rmc-aslot-stock");
    }), handle: false)).Bind(CMKeyFunctions.RMCActivateAttachableUnderbarrel, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.ToggleAttachable(attachedEntity.GetValueOrDefault(), "rmc-aslot-underbarrel");
    }), handle: false)).Bind(CMKeyFunctions.RMCFieldStripHeldItem, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.FieldStripHeldItem(attachedEntity.GetValueOrDefault());
    }), handle: false)).Register<AttachableHolderSystem>();
  }

  public override void Shutdown() => CommandBinds.Unregister<AttachableHolderSystem>();

  private void OnHolderMapInit(Entity<AttachableHolderComponent> holder, ref MapInitEvent args)
  {
    TransformComponent containerXform = this.Transform(holder.Owner);
    EntityCoordinates coordinates = new EntityCoordinates(holder.Owner, Vector2.Zero);
    bool flag = this._random.Prob(holder.Comp.RandomAttachmentChance);
    foreach (string key in holder.Comp.Slots.Keys)
    {
      AttachableSlot slot = holder.Comp.Slots[key];
      EntProtoId<AttachableComponent>? nullable1 = slot.StartingAttachable;
      if (flag)
      {
        List<EntProtoId<AttachableComponent>> random = slot.Random;
        if (random != null && random.Count > 0 && this._random.Prob(slot.RandomChance))
          nullable1 = new EntProtoId<AttachableComponent>?(RandomExtensions.Pick<EntProtoId<AttachableComponent>>(this._random, (IReadOnlyList<EntProtoId<AttachableComponent>>) random));
      }
      if (nullable1.HasValue)
      {
        ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) holder, key);
        container.OccludesLight = false;
        EntProtoId<AttachableComponent>? nullable2 = nullable1;
        this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) this.Spawn(nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null, coordinates), (BaseContainer) container, containerXform);
      }
    }
    this.Dirty<AttachableHolderComponent>(holder);
  }

  private void OnAttachableHolderBeforeRangedInteract(
    Entity<AttachableHolderComponent> holder,
    ref BeforeRangedInteractEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? supercedingAttachable = holder.Comp.SupercedingAttachable;
    if (!supercedingAttachable.HasValue)
      return;
    EntityUid valueOrDefault = supercedingAttachable.GetValueOrDefault();
    BeforeRangedInteractEvent args1 = new BeforeRangedInteractEvent(args.User, valueOrDefault, args.Target, args.ClickLocation, args.CanReach);
    this.RaiseLocalEvent<BeforeRangedInteractEvent>(valueOrDefault, args1);
    if (!args1.Handled)
      return;
    args.Handled = true;
  }

  private void OnAttachableHolderInteractUsing(
    Entity<AttachableHolderComponent> holder,
    ref InteractUsingEvent args)
  {
    if (this.CanAttach(holder, args.Used))
    {
      this.StartAttach(holder, args.Used, args.User);
      args.Handled = true;
    }
    if (!holder.Comp.SupercedingAttachable.HasValue)
      return;
    InteractUsingEvent args1 = new InteractUsingEvent(args.User, args.Used, holder.Comp.SupercedingAttachable.Value, args.ClickLocation);
    this.RaiseLocalEvent<InteractUsingEvent>(holder.Comp.SupercedingAttachable.Value, args1);
    if (args1.Handled)
    {
      args.Handled = true;
    }
    else
    {
      AfterInteractEvent args2 = new AfterInteractEvent(args.User, args.Used, new EntityUid?(holder.Comp.SupercedingAttachable.Value), args.ClickLocation, true);
      this.RaiseLocalEvent<AfterInteractEvent>(args.Used, args2);
      if (!args2.Handled)
        return;
      args.Handled = true;
    }
  }

  private void OnAttachableHolderAfterInteract(
    Entity<AttachableHolderComponent> holder,
    ref AfterInteractEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? supercedingAttachable = holder.Comp.SupercedingAttachable;
    if (!supercedingAttachable.HasValue)
      return;
    EntityUid valueOrDefault = supercedingAttachable.GetValueOrDefault();
    AfterInteractEvent args1 = new AfterInteractEvent(args.User, valueOrDefault, args.Target, args.ClickLocation, args.CanReach);
    this.RaiseLocalEvent<AfterInteractEvent>(valueOrDefault, args1);
    if (!args1.Handled)
      return;
    args.Handled = true;
  }

  private void OnAttachableHolderInteractInWorld(
    Entity<AttachableHolderComponent> holder,
    ref ActivateInWorldEvent args)
  {
    if (args.Handled || !holder.Comp.SupercedingAttachable.HasValue)
      return;
    ActivateInWorldEvent args1 = new ActivateInWorldEvent(args.User, holder.Comp.SupercedingAttachable.Value, args.Complex);
    this.RaiseLocalEvent<ActivateInWorldEvent>(holder.Comp.SupercedingAttachable.Value, args1);
    args.Handled = args1.Handled;
  }

  private void OnAttachableHolderUniqueAction(
    Entity<AttachableHolderComponent> holder,
    ref UniqueActionEvent args)
  {
    if (!holder.Comp.SupercedingAttachable.HasValue || args.Handled)
      return;
    this.RaiseLocalEvent<UniqueActionEvent>(holder.Comp.SupercedingAttachable.Value, new UniqueActionEvent(args.UserUid));
    args.Handled = true;
  }

  private void OnHolderWielded(Entity<AttachableHolderComponent> holder, ref ItemWieldedEvent args)
  {
    this.AlterAllAttachables(holder, AttachableAlteredType.Wielded);
  }

  private void OnHolderUnwielded(
    Entity<AttachableHolderComponent> holder,
    ref ItemUnwieldedEvent args)
  {
    this.AlterAllAttachables(holder, AttachableAlteredType.Unwielded);
  }

  private void OnAttachableHolderDetachMessage(
    EntityUid holderUid,
    AttachableHolderComponent holderComponent,
    AttachableHolderDetachMessage args)
  {
    this.StartDetach((Entity<AttachableHolderComponent>) (holderUid, holderComponent), args.Slot, args.Actor);
  }

  private void OnAttachableHolderGetVerbs(
    Entity<AttachableHolderComponent> holder,
    ref GetVerbsEvent<EquipmentVerb> args)
  {
    if (this.HasComp<XenoComponent>(args.User))
      return;
    this.EnsureSlots(holder);
    EntityUid userUid = args.User;
    foreach (string key in holder.Comp.Slots.Keys)
    {
      string slotId = key;
      BaseContainer container;
      if (this._container.TryGetContainer(holder.Owner, slotId, out container))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
        {
          EntityUid contained = containedEntity;
          AttachableToggleableComponent comp1;
          TransformComponent comp2;
          if (this.TryComp<AttachableToggleableComponent>(contained, out comp1) && (!comp1.UserOnly || this.TryComp(holder.Owner, out comp2) && comp2.ParentUid.Valid && !(comp2.ParentUid != userUid)))
          {
            EquipmentVerb equipmentVerb1 = new EquipmentVerb();
            equipmentVerb1.Text = comp1.ActionName;
            equipmentVerb1.IconEntity = new NetEntity?(this.GetNetEntity(contained));
            equipmentVerb1.Act = (Action) (() =>
            {
              AttachableToggleStartedEvent args1 = new AttachableToggleStartedEvent(holder.Owner, userUid, slotId);
              this.RaiseLocalEvent<AttachableToggleStartedEvent>(contained, ref args1);
            });
            EquipmentVerb equipmentVerb2 = equipmentVerb1;
            args.Verbs.Add(equipmentVerb2);
          }
        }
      }
    }
  }

  private void OnAttachableHolderAttachToSlotMessage(
    EntityUid holderUid,
    AttachableHolderComponent holderComponent,
    AttachableHolderAttachToSlotMessage args)
  {
    HandsComponent comp;
    this.TryComp<HandsComponent>(args.Actor, out comp);
    if (comp == null)
      return;
    EntityUid? nullable;
    this._hands.TryGetActiveItem((Entity<HandsComponent>) (args.Actor, comp), out nullable);
    if (!nullable.HasValue)
      return;
    this.StartAttach((Entity<AttachableHolderComponent>) (holderUid, holderComponent), nullable.Value, args.Actor, args.Slot);
  }

  private void OnAttachableHolderUiOpened(
    EntityUid holderUid,
    AttachableHolderComponent holderComponent,
    BoundUIOpenedEvent args)
  {
    this.UpdateStripUi(holderUid);
  }

  public void StartAttach(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    EntityUid userUid,
    string slotId = "")
  {
    if (this.HasComp<XenoComponent>(userUid))
      return;
    List<string> validSlots = this.GetValidSlots(holder, attachableUid);
    if (validSlots.Count == 0)
      return;
    if (string.IsNullOrEmpty(slotId))
    {
      if (validSlots.Count > 1)
      {
        UserInterfaceComponent comp;
        this.TryComp<UserInterfaceComponent>(holder.Owner, out comp);
        this._ui.OpenUi((Entity<UserInterfaceComponent>) (holder.Owner, comp), (Enum) AttachmentUI.ChooseSlotKey, new EntityUid?(userUid));
        AttachableHolderChooseSlotUserInterfaceState state = new AttachableHolderChooseSlotUserInterfaceState(validSlots);
        this._ui.SetUiState((Entity<UserInterfaceComponent>) holder.Owner, (Enum) AttachmentUI.ChooseSlotKey, (BoundUserInterfaceState) state);
        return;
      }
      slotId = validSlots[0];
    }
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, this.Comp<AttachableComponent>(attachableUid).AttachDoAfter, (DoAfterEvent) new AttachableAttachDoAfterEvent(slotId), new EntityUid?((EntityUid) holder), new EntityUid?(holder.Owner), new EntityUid?(attachableUid))
    {
      NeedHand = true,
      BreakOnMove = true
    });
  }

  private void OnAttachDoAfter(
    EntityUid uid,
    AttachableHolderComponent component,
    AttachableAttachDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault1 = target.GetValueOrDefault();
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault2 = used.GetValueOrDefault();
    AttachableHolderComponent comp;
    if (!this.TryComp<AttachableHolderComponent>(args.Target, out comp) || !this.HasComp<AttachableComponent>(args.Used) || !this.Attach((Entity<AttachableHolderComponent>) (valueOrDefault1, comp), valueOrDefault2, args.User, args.SlotId))
      return;
    args.Handled = true;
  }

  public bool Attach(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    EntityUid userUid,
    string slotId = "")
  {
    if (!this.CanAttach(holder, attachableUid, ref slotId))
      return false;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) holder, slotId);
    container.OccludesLight = false;
    if (container.Count > 0 && !this.Detach(holder, container.ContainedEntities[0], userUid, slotId) || !this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) attachableUid, (BaseContainer) container))
      return false;
    if (this._hands.IsHolding((Entity<HandsComponent>) userUid, new EntityUid?(holder.Owner)))
    {
      GrantAttachableActionsEvent args = new GrantAttachableActionsEvent(userUid);
      this.RaiseLocalEvent<GrantAttachableActionsEvent>(attachableUid, ref args);
    }
    this.Dirty<AttachableHolderComponent>(holder);
    this._audio.PlayPredicted(this.Comp<AttachableComponent>(attachableUid).AttachSound, (EntityUid) holder, new EntityUid?(userUid));
    return true;
  }

  private void OnAttached(
    Entity<AttachableHolderComponent> holder,
    ref EntInsertedIntoContainerMessage args)
  {
    if (!this.TryComp<AttachableComponent>(args.Entity, out AttachableComponent _) || !holder.Comp.Slots.ContainsKey(args.Container.ID))
      return;
    this.UpdateStripUi(holder.Owner, holder.Comp);
    AttachableAlteredEvent args1 = new AttachableAlteredEvent(holder.Owner, AttachableAlteredType.Attached);
    this.RaiseLocalEvent<AttachableAlteredEvent>(args.Entity, ref args1);
    AttachableHolderAttachablesAlteredEvent args2 = new AttachableHolderAttachablesAlteredEvent(args.Entity, args.Container.ID, AttachableAlteredType.Attached);
    this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>((EntityUid) holder, ref args2);
  }

  public void StartDetach(
    Entity<AttachableHolderComponent> holder,
    string slotId,
    EntityUid userUid)
  {
    Entity<AttachableComponent> attachable;
    if (!this.TryGetAttachable(holder, slotId, out attachable) || !holder.Comp.Slots.ContainsKey(slotId) || holder.Comp.Slots[slotId].Locked)
      return;
    this.StartDetach(holder, attachable.Owner, userUid);
  }

  public void StartDetach(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    EntityUid userUid)
  {
    if (this.HasComp<XenoComponent>(userUid))
      return;
    float attachDoAfter = this.Comp<AttachableComponent>(attachableUid).AttachDoAfter;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, attachDoAfter, (DoAfterEvent) new AttachableDetachDoAfterEvent(), new EntityUid?((EntityUid) holder), new EntityUid?(holder.Owner), new EntityUid?(attachableUid))
    {
      NeedHand = true,
      BreakOnMove = true
    });
  }

  private void OnDetachDoAfter(
    EntityUid uid,
    AttachableHolderComponent component,
    AttachableDetachDoAfterEvent args)
  {
    AttachableHolderComponent comp;
    if (args.Cancelled || args.Handled || !args.Target.HasValue || !args.Used.HasValue || !this.TryComp<AttachableHolderComponent>(args.Target, out comp) || !this.HasComp<AttachableComponent>(args.Used) || !this.Detach((Entity<AttachableHolderComponent>) (args.Target.Value, comp), args.Used.Value, args.User))
      return;
    args.Handled = true;
  }

  public bool Detach(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    EntityUid userUid,
    string? slotId = null)
  {
    BaseContainer container;
    Entity<AttachableComponent> attachable;
    if (this.TerminatingOrDeleted((EntityUid) holder) || !holder.Comp.Running || string.IsNullOrEmpty(slotId) && !this.TryGetSlotId(holder.Owner, attachableUid, out slotId) || !this._container.TryGetContainer((EntityUid) holder, slotId, out container) || container.Count <= 0 || !this.TryGetAttachable(holder, slotId, out attachable) || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) attachable.Owner, container, force: true))
      return false;
    this.UpdateStripUi(holder.Owner, holder.Comp);
    AttachableAlteredEvent args1 = new AttachableAlteredEvent(holder.Owner, AttachableAlteredType.Detached, new EntityUid?(userUid));
    this.RaiseLocalEvent<AttachableAlteredEvent>(attachableUid, ref args1);
    AttachableHolderAttachablesAlteredEvent args2 = new AttachableHolderAttachablesAlteredEvent(attachableUid, slotId, AttachableAlteredType.Detached);
    this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref args2);
    RemoveAttachableActionsEvent args3 = new RemoveAttachableActionsEvent(userUid);
    this.RaiseLocalEvent<RemoveAttachableActionsEvent>(attachableUid, ref args3);
    this._audio.PlayPredicted(this.Comp<AttachableComponent>(attachableUid).DetachSound, (EntityUid) holder, new EntityUid?(userUid));
    this.Dirty<AttachableHolderComponent>(holder);
    this._hands.TryPickupAnyHand(userUid, (EntityUid) attachable);
    return true;
  }

  private bool CanAttach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid)
  {
    string slotId = "";
    return this.CanAttach(holder, attachableUid, ref slotId);
  }

  private bool CanAttach(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    ref string slotId)
  {
    if (!this.HasComp<AttachableComponent>(attachableUid))
      return false;
    if (!string.IsNullOrWhiteSpace(slotId))
      return this._whitelist.IsWhitelistPass(holder.Comp.Slots[slotId].Whitelist, attachableUid);
    foreach (string key in holder.Comp.Slots.Keys)
    {
      if (this._whitelist.IsWhitelistPass(holder.Comp.Slots[key].Whitelist, attachableUid))
      {
        slotId = key;
        return true;
      }
    }
    return false;
  }

  private Dictionary<string, (string?, bool)> GetSlotsForStripUi(
    Entity<AttachableHolderComponent> holder)
  {
    Dictionary<string, (string, bool)> slotsForStripUi = new Dictionary<string, (string, bool)>();
    Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> entityQuery = this.GetEntityQuery<MetaDataComponent>();
    foreach (string key in holder.Comp.Slots.Keys)
    {
      Entity<AttachableComponent> attachable;
      MetaDataComponent component;
      if (this.TryGetAttachable(holder, key, out attachable) && entityQuery.TryGetComponent(attachable.Owner, out component))
        slotsForStripUi.Add(key, (component.EntityName, holder.Comp.Slots[key].Locked));
      else
        slotsForStripUi.Add(key, ((string) null, holder.Comp.Slots[key].Locked));
    }
    return slotsForStripUi;
  }

  public bool TryGetAttachable(
    Entity<AttachableHolderComponent> holder,
    string slotId,
    out Entity<AttachableComponent> attachable)
  {
    attachable = new Entity<AttachableComponent>();
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) holder, slotId, out container) || container.Count <= 0)
      return false;
    EntityUid containedEntity = container.ContainedEntities[0];
    AttachableComponent comp;
    if (!this.TryComp<AttachableComponent>(containedEntity, out comp))
      return false;
    attachable = (Entity<AttachableComponent>) (containedEntity, comp);
    return true;
  }

  private void UpdateStripUi(EntityUid holderUid, AttachableHolderComponent? holderComponent = null)
  {
    if (!this.Resolve<AttachableHolderComponent>(holderUid, ref holderComponent))
      return;
    AttachableHolderStripUserInterfaceState state = new AttachableHolderStripUserInterfaceState(this.GetSlotsForStripUi((Entity<AttachableHolderComponent>) (holderUid, holderComponent)));
    this._ui.SetUiState((Entity<UserInterfaceComponent>) holderUid, (Enum) AttachmentUI.StripKey, (BoundUserInterfaceState) state);
  }

  private void EnsureSlots(Entity<AttachableHolderComponent> holder)
  {
    foreach (string key in holder.Comp.Slots.Keys)
      this._container.EnsureContainer<ContainerSlot>((EntityUid) holder, key).OccludesLight = false;
  }

  private List<string> GetValidSlots(
    Entity<AttachableHolderComponent> holder,
    EntityUid attachableUid,
    bool ignoreLock = false)
  {
    List<string> validSlots = new List<string>();
    if (!this.HasComp<AttachableComponent>(attachableUid))
      return validSlots;
    foreach (string key in holder.Comp.Slots.Keys)
    {
      if (this._whitelist.IsWhitelistPass(holder.Comp.Slots[key].Whitelist, attachableUid) && (!ignoreLock || !holder.Comp.Slots[key].Locked))
        validSlots.Add(key);
    }
    return validSlots;
  }

  private void ToggleAttachable(EntityUid userUid, string slotId)
  {
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) userUid);
    if (!activeItem.HasValue)
      return;
    EntityUid valueOrDefault = activeItem.GetValueOrDefault();
    AttachableHolderComponent comp;
    BaseContainer container;
    if (!this.TryComp<AttachableHolderComponent>(valueOrDefault, out comp) || !comp.Running || !this._actionBlocker.CanInteract(userUid, new EntityUid?(valueOrDefault)) || !this._container.TryGetContainer(valueOrDefault, slotId, out container) || container.Count <= 0)
      return;
    EntityUid containedEntity = container.ContainedEntities[0];
    if (!this.HasComp<AttachableToggleableComponent>(containedEntity))
      return;
    AttachableToggleStartedEvent args = new AttachableToggleStartedEvent(valueOrDefault, userUid, slotId);
    this.RaiseLocalEvent<AttachableToggleStartedEvent>(containedEntity, ref args);
  }

  private void FieldStripHeldItem(EntityUid userUid)
  {
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) userUid);
    if (!activeItem.HasValue)
      return;
    EntityUid valueOrDefault = activeItem.GetValueOrDefault();
    AttachableHolderComponent comp;
    if (!this.TryComp<AttachableHolderComponent>(valueOrDefault, out comp) || !comp.Running || !this._actionBlocker.CanInteract(userUid, new EntityUid?(valueOrDefault)))
      return;
    foreach (Verb localVerb in this._verbSystem.GetLocalVerbs(valueOrDefault, userUid, typeof (Verb)))
    {
      if (localVerb.Text.Equals(this.Loc.GetString("rmc-verb-strip-attachables")))
      {
        this._verbSystem.ExecuteVerb(localVerb, userUid, valueOrDefault);
        break;
      }
    }
  }

  public void SetSupercedingAttachable(
    Entity<AttachableHolderComponent> holder,
    EntityUid? supercedingAttachable)
  {
    holder.Comp.SupercedingAttachable = supercedingAttachable;
    this.Dirty<AttachableHolderComponent>(holder);
  }

  public bool TryGetInhandSupercedingGun(
    EntityUid user,
    out EntityUid attachable,
    [NotNullWhen(true)] out GunComponent? gunComp)
  {
    attachable = new EntityUid();
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) user);
    AttachableHolderComponent comp;
    if (!activeItem.HasValue || !this.TryComp<AttachableHolderComponent>(activeItem.GetValueOrDefault(), out comp) || !comp.SupercedingAttachable.HasValue)
    {
      gunComp = (GunComponent) null;
      return false;
    }
    if (!this.TryComp<GunComponent>(comp.SupercedingAttachable, out gunComp))
      return false;
    attachable = comp.SupercedingAttachable.Value;
    return true;
  }

  public bool TryGetSlotId(EntityUid holderUid, EntityUid attachableUid, [NotNullWhen(true)] out string? slotId)
  {
    slotId = (string) null;
    AttachableHolderComponent comp;
    if (!this.TryComp<AttachableHolderComponent>(holderUid, out comp) || !this.TryComp<AttachableComponent>(attachableUid, out AttachableComponent _))
      return false;
    foreach (string key in comp.Slots.Keys)
    {
      BaseContainer container;
      if (this._container.TryGetContainer(holderUid, key, out container) && container.Count > 0 && !(container.ContainedEntities[0] != attachableUid))
      {
        slotId = key;
        return true;
      }
    }
    return false;
  }

  public bool HasSlot(Entity<AttachableHolderComponent?> holder, string slotId)
  {
    if (holder.Comp == null)
    {
      AttachableHolderComponent comp;
      if (!this.TryComp<AttachableHolderComponent>(holder.Owner, out comp))
        return false;
      holder.Comp = comp;
    }
    return holder.Comp.Slots.ContainsKey(slotId);
  }

  public bool TryGetHolder(EntityUid attachable, [NotNullWhen(true)] out EntityUid? holderUid)
  {
    TransformComponent comp;
    if (!this.TryComp(attachable, out comp) || !comp.ParentUid.Valid || !this.HasComp<AttachableHolderComponent>(comp.ParentUid))
    {
      holderUid = new EntityUid?();
      return false;
    }
    holderUid = new EntityUid?(comp.ParentUid);
    return true;
  }

  public bool TryGetUser(EntityUid attachable, [NotNullWhen(true)] out EntityUid? userUid)
  {
    userUid = new EntityUid?();
    EntityUid? holderUid;
    TransformComponent comp;
    if (!this.TryGetHolder(attachable, out holderUid) || !this.TryComp(holderUid, out comp) || !comp.ParentUid.Valid)
      return false;
    userUid = new EntityUid?(comp.ParentUid);
    return true;
  }

  public void RelayEvent<T>(Entity<AttachableHolderComponent> holder, ref T args) where T : notnull
  {
    AttachableRelayedEvent<T> args1 = new AttachableRelayedEvent<T>(args, holder.Owner);
    foreach (string key in holder.Comp.Slots.Keys)
    {
      BaseContainer container;
      if (this._container.TryGetContainer((EntityUid) holder, key, out container))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
          this.RaiseLocalEvent<AttachableRelayedEvent<T>>(containedEntity, args1);
      }
    }
    args = args1.Args;
  }

  private void AlterAllAttachables(
    Entity<AttachableHolderComponent> holder,
    AttachableAlteredType alteration)
  {
    foreach (string key in holder.Comp.Slots.Keys)
    {
      BaseContainer container;
      if (this._container.TryGetContainer((EntityUid) holder, key, out container) && container.Count > 0)
      {
        AttachableAlteredEvent args = new AttachableAlteredEvent(holder.Owner, alteration);
        this.RaiseLocalEvent<AttachableAlteredEvent>(container.ContainedEntities[0], ref args);
      }
    }
  }
}
