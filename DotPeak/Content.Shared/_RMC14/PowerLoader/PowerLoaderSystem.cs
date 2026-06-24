// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.PowerLoader.PowerLoaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Fabricator;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.PowerLoader.Events;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.PowerLoader;

public sealed class PowerLoaderSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedVirtualItemSystem _virtualItem;
  [Dependency]
  private TagSystem _tag;
  private static readonly EntProtoId DefaultHandVisual = (EntProtoId) "RMCVirtualDropshipGearRight";
  private Robust.Shared.GameObjects.EntityQuery<PowerLoaderGrabbableComponent> _powerLoaderGrabbableQuery;

  public override void Initialize()
  {
    this._powerLoaderGrabbableQuery = this.GetEntityQuery<PowerLoaderGrabbableComponent>();
    this.SubscribeLocalEvent<ItemComponent, AfterInteractEvent>(new EntityEventRefHandler<ItemComponent, AfterInteractEvent>(this.OnItemAfterInteract));
    this.SubscribeLocalEvent<PowerLoaderComponent, MapInitEvent>(new EntityEventRefHandler<PowerLoaderComponent, MapInitEvent>(this.OnPowerLoaderMapInit));
    this.SubscribeLocalEvent<PowerLoaderComponent, ComponentRemove>(new EntityEventRefHandler<PowerLoaderComponent, ComponentRemove>(this.OnPowerLoaderRemove));
    this.SubscribeLocalEvent<PowerLoaderComponent, EntityTerminatingEvent>(new EntityEventRefHandler<PowerLoaderComponent, EntityTerminatingEvent>(this.OnPowerLoaderTerminating));
    this.SubscribeLocalEvent<PowerLoaderComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<PowerLoaderComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<PowerLoaderComponent, StrapAttemptEvent>(new EntityEventRefHandler<PowerLoaderComponent, StrapAttemptEvent>(this.OnStrapAttempt));
    this.SubscribeLocalEvent<PowerLoaderComponent, StrappedEvent>(new EntityEventRefHandler<PowerLoaderComponent, StrappedEvent>(this.OnStrapped));
    this.SubscribeLocalEvent<PowerLoaderComponent, UnstrappedEvent>(new EntityEventRefHandler<PowerLoaderComponent, UnstrappedEvent>(this.OnUnstrapped));
    this.SubscribeLocalEvent<PowerLoaderComponent, PowerLoaderGrabDoAfterEvent>(new EntityEventRefHandler<PowerLoaderComponent, PowerLoaderGrabDoAfterEvent>(this.OnGrabDoAfter));
    this.SubscribeLocalEvent<PowerLoaderComponent, GetUsedEntityEvent>(new EntityEventRefHandler<PowerLoaderComponent, GetUsedEntityEvent>(this.OnGetUsedEntity), after: new Type[1]
    {
      typeof (SharedHandsSystem)
    });
    this.SubscribeLocalEvent<PowerLoaderComponent, UserActivateInWorldEvent>(new EntityEventRefHandler<PowerLoaderComponent, UserActivateInWorldEvent>(this.OnUserGrab));
    this.SubscribeLocalEvent<PowerLoaderComponent, DestructionEventArgs>(new EntityEventRefHandler<PowerLoaderComponent, DestructionEventArgs>(this.OnDestruction));
    this.SubscribeLocalEvent<PowerLoaderComponent, DidEquipHandEvent>(new EntityEventRefHandler<PowerLoaderComponent, DidEquipHandEvent>(this.OnHandsChanged<DidEquipHandEvent>));
    this.SubscribeLocalEvent<PowerLoaderComponent, DidUnequipHandEvent>(new EntityEventRefHandler<PowerLoaderComponent, DidUnequipHandEvent>(this.OnHandsChanged<DidUnequipHandEvent>));
    this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, PickupAttemptEvent>(new EntityEventRefHandler<PowerLoaderGrabbableComponent, PickupAttemptEvent>(this.OnGrabbablePickupAttempt));
    this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, AfterInteractEvent>(new EntityEventRefHandler<PowerLoaderGrabbableComponent, AfterInteractEvent>(this.OnGrabbableAfterInteract));
    this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, CombatModeShouldHandInteractEvent>(new EntityEventRefHandler<PowerLoaderGrabbableComponent, CombatModeShouldHandInteractEvent>(this.OnGrababbleShouldInteract));
    this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<PowerLoaderGrabbableComponent, BeforeRangedInteractEvent>(this.OnGrabbableBeforeRangedInteract));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, ActivateInWorldEvent>(this.OnPointActivateInWorld));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, ActivateInWorldEvent>(this.OnPointActivateInWorld));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, ActivateInWorldEvent>(this.OnEngineActivateInWorld));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, ActivateInWorldEvent>(this.OnEngineActivateInWorld));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, DropshipDetachDoAfterEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, DropshipDetachDoAfterEvent>(this.OnDropshipDetach));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipDetachDoAfterEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, DropshipDetachDoAfterEvent>(this.OnDropshipDetach));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, DropshipDetachDoAfterEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, DropshipDetachDoAfterEvent>(this.OnEngineDetach));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropshipDetachDoAfterEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropshipDetachDoAfterEvent>(this.OnEngineDetach));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, GetAttachmentSlotEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, GetAttachmentSlotEvent>(this.OnGetSlot));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, GetAttachmentSlotEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, GetAttachmentSlotEvent>(this.OnGetSlot));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, GetAttachmentSlotEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, GetAttachmentSlotEvent>(this.OnGetSlot));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, GetAttachmentSlotEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, GetAttachmentSlotEvent>(this.OnGetSlot));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, DropshipAttachDoAfterEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, DropshipAttachDoAfterEvent>(this.OnDropshipAttach));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipAttachDoAfterEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, DropshipAttachDoAfterEvent>(this.OnDropshipAttach));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, DropshipAttachDoAfterEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, DropshipAttachDoAfterEvent>(this.OnDropshipAttach));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropshipAttachDoAfterEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropshipAttachDoAfterEvent>(this.OnDropshipAttach));
    this.SubscribeLocalEvent<DropshipFabricatorPrintableComponent, PowerLoaderInteractEvent>(new EntityEventRefHandler<DropshipFabricatorPrintableComponent, PowerLoaderInteractEvent>(this.OnDropshipPartPowerLoaderInteract));
    this.SubscribeLocalEvent<ActivePowerLoaderPilotComponent, PreventCollideEvent>(new EntityEventRefHandler<ActivePowerLoaderPilotComponent, PreventCollideEvent>(this.OnActivePilotPreventCollide));
    this.SubscribeLocalEvent<ActivePowerLoaderPilotComponent, KnockedDownEvent>(new EntityEventRefHandler<ActivePowerLoaderPilotComponent, KnockedDownEvent>(this.OnActivePilotStunned<KnockedDownEvent>));
    this.SubscribeLocalEvent<ActivePowerLoaderPilotComponent, StunnedEvent>(new EntityEventRefHandler<ActivePowerLoaderPilotComponent, StunnedEvent>(this.OnActivePilotStunned<StunnedEvent>));
    this.SubscribeLocalEvent<ActivePowerLoaderPilotComponent, MobStateChangedEvent>(new EntityEventRefHandler<ActivePowerLoaderPilotComponent, MobStateChangedEvent>(this.OnActivePilotMobStateChanged));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DropshipWeaponPointComponent, EntRemovedFromContainerMessage>(this.OnWeaponPointContainerChanged));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DropshipUtilityPointComponent, EntRemovedFromContainerMessage>(this.OnUtilityPointContainerChanged));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DropshipEnginePointComponent, EntRemovedFromContainerMessage>(this.OnEnginePointContainerChanged));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, EntRemovedFromContainerMessage>(this.OnElectronicPointContainerChanged));
    this.SubscribeLocalEvent<ActivePowerLoaderPilotComponent, CatchAttemptEvent>(new EntityEventRefHandler<ActivePowerLoaderPilotComponent, CatchAttemptEvent>(this.OnPowerLoaderPilotCatchAttempt));
    this.SubscribeLocalEvent<PowerLoaderComponent, BeforeMeltedEvent>(new EntityEventRefHandler<PowerLoaderComponent, BeforeMeltedEvent>(this.PowerLoaderBeforeMelted));
  }

  private void OnItemAfterInteract(Entity<ItemComponent> ent, ref AfterInteractEvent args)
  {
    BaseContainer container;
    PowerLoaderComponent comp1;
    HandsComponent comp2;
    if (args.Handled || this._powerLoaderGrabbableQuery.HasComp((EntityUid) ent) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ent.Owner, out container) || !this.TryComp<PowerLoaderComponent>(container.Owner, out comp1) || !this.TryComp<HandsComponent>(container.Owner, out comp2) || this._hands.EnumerateHeld((Entity<HandsComponent>) (container.Owner, comp2)).All<EntityUid>((Func<EntityUid, bool>) (held => held != ent.Owner)) || !this.TryDropLoaderHeld((Entity<PowerLoaderComponent>) (container.Owner, comp1), args.ClickLocation, args.Used))
      return;
    args.Handled = true;
  }

  private void OnPowerLoaderMapInit(Entity<PowerLoaderComponent> ent, ref MapInitEvent args)
  {
    this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.VirtualContainerId);
  }

  private void OnPowerLoaderRemove(Entity<PowerLoaderComponent> ent, ref ComponentRemove args)
  {
    this.RemoveLoader(ent);
  }

  private void OnPowerLoaderTerminating(
    Entity<PowerLoaderComponent> ent,
    ref EntityTerminatingEvent args)
  {
    this.RemoveLoader(ent);
  }

  private void OnRefreshSpeed(
    Entity<PowerLoaderComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>((EntityUid) ent, out comp))
      return;
    int num1 = 0;
    foreach (EntityUid buckledEntity in comp.BuckledEntities)
    {
      int skill = this._skills.GetSkill((Entity<SkillsComponent>) buckledEntity, ent.Comp.SpeedSkill);
      if (skill > num1)
        num1 = skill;
    }
    if (num1 <= 0)
      return;
    float num2 = ent.Comp.SpeedPerSkill * (float) num1;
    args.ModifySpeed(num2, num2);
  }

  private void OnStrapAttempt(Entity<PowerLoaderComponent> ent, ref StrapAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    Entity<BuckleComponent> buckle = args.Buckle;
    if (!this._skills.HasSkills((Entity<SkillsComponent>) buckle.Owner, ent.Comp.Skills))
    {
      if (args.Popup)
        this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-operate", ("target", (object) ent)), (EntityUid) buckle, args.User);
      args.Cancelled = true;
    }
    else
    {
      if (this._hands.CountFreeHands((Entity<HandsComponent>) buckle.Owner) >= 2)
        return;
      if (args.Popup)
        this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-hands-occupied", ("mech", (object) ent)), (EntityUid) buckle, args.User);
      args.Cancelled = true;
    }
  }

  private void OnStrapped(Entity<PowerLoaderComponent> ent, ref StrappedEvent args)
  {
    Entity<BuckleComponent> buckle = args.Buckle;
    InteractionRelayComponent component = this.EnsureComp<InteractionRelayComponent>((EntityUid) buckle);
    this.EnsureComp<ActivePowerLoaderPilotComponent>((EntityUid) buckle);
    this._mover.SetRelay((EntityUid) buckle, (EntityUid) ent);
    this._interaction.SetRelay((EntityUid) buckle, new EntityUid?((EntityUid) ent), component);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    this.SyncHands(ent);
  }

  private void OnUnstrapped(Entity<PowerLoaderComponent> ent, ref UnstrappedEvent args)
  {
    Entity<BuckleComponent> buckle = args.Buckle;
    this.RemCompDeferred<ActivePowerLoaderPilotComponent>((EntityUid) buckle);
    this.RemCompDeferred<RelayInputMoverComponent>((EntityUid) buckle);
    this.RemCompDeferred<InteractionRelayComponent>((EntityUid) buckle);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    this.DeleteVirtuals(ent, (EntityUid) buckle);
    if (ent.Comp.DoAfter == null || !this._doAfter.IsRunning(new DoAfterId?(ent.Comp.DoAfter.Id)))
      return;
    this._doAfter.Cancel(new DoAfterId?(ent.Comp.DoAfter.Id));
  }

  private void OnGrabDoAfter(Entity<PowerLoaderComponent> ent, ref PowerLoaderGrabDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this.PickUp(ent, valueOrDefault);
  }

  private void OnGetUsedEntity(Entity<PowerLoaderComponent> ent, ref GetUsedEntityEvent args)
  {
    args.Used = new EntityUid?();
    foreach (EntityUid uid1 in this.GetBuckled((EntityUid) ent))
    {
      HandsComponent comp1;
      EntityUid? uid2;
      VirtualItemComponent comp2;
      PowerLoaderVirtualItemComponent comp3;
      if (this.TryComp<HandsComponent>(uid1, out comp1) && this._hands.TryGetActiveItem((Entity<HandsComponent>) (uid1, comp1), out uid2) && this.TryComp<VirtualItemComponent>(uid2, out comp2) && this.TryComp<PowerLoaderVirtualItemComponent>(comp2.BlockingEntity, out comp3))
      {
        foreach (EntityUid entityUid in this._hands.EnumerateHeld((Entity<HandsComponent>) ent.Owner))
        {
          EntityUid? grabbed = comp3.Grabbed;
          if ((grabbed.HasValue ? (entityUid == grabbed.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            args.Used = comp3.Grabbed;
            return;
          }
        }
        args.Used = new EntityUid?();
        break;
      }
    }
  }

  private void OnUserGrab(Entity<PowerLoaderComponent> ent, ref UserActivateInWorldEvent args)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>((EntityUid) ent, out comp))
      return;
    PowerLoaderGrabEvent args1 = new PowerLoaderGrabEvent((EntityUid) ent, args.Target, comp.BuckledEntities);
    this.RaiseLocalEvent<PowerLoaderGrabEvent>(args.Target, ref args1);
    EntityUid? nullable = args1.ToGrab;
    if (nullable.HasValue)
    {
      Entity<PowerLoaderComponent> loader = ent;
      nullable = args1.ToGrab;
      EntityUid target = nullable.Value;
      this.PickUp(loader, target);
    }
    else
    {
      TimeSpan delay1;
      if (!this.CanPickupPopup(ent, (Entity<PowerLoaderGrabbableComponent>) args.Target, out delay1))
        return;
      PowerLoaderGrabDoAfterEvent grabDoAfterEvent = new PowerLoaderGrabDoAfterEvent();
      EntityManager entityManager = this.EntityManager;
      EntityUid user = (EntityUid) ent;
      TimeSpan delay2 = delay1;
      PowerLoaderGrabDoAfterEvent @event = grabDoAfterEvent;
      EntityUid? eventTarget = new EntityUid?((EntityUid) ent);
      EntityUid? target = new EntityUid?(args.Target);
      nullable = new EntityUid?();
      EntityUid? used = nullable;
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, delay2, (DoAfterEvent) @event, eventTarget, target, used)
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent,
        DistanceThreshold = new float?(2.5f)
      }))
        return;
      ent.Comp.DoAfter = grabDoAfterEvent.DoAfter;
    }
  }

  private void OnDestruction(Entity<PowerLoaderComponent> ent, ref DestructionEventArgs args)
  {
    foreach (EntityUid entity in this._hands.EnumerateHeld((Entity<HandsComponent>) ent.Owner).ToList<EntityUid>())
      this._hands.TryDrop((Entity<HandsComponent>) ent.Owner, entity);
  }

  private void OnHandsChanged<T>(Entity<PowerLoaderComponent> ent, ref T args)
  {
    this.SyncHands(ent);
  }

  private void OnPointActivateInWorld(
    Entity<DropshipWeaponPointComponent> ent,
    ref ActivateInWorldEvent args)
  {
    PowerLoaderComponent comp;
    if (!this.TryComp<PowerLoaderComponent>(args.User, out comp))
      return;
    args.Handled = true;
    Entity<PowerLoaderComponent> user = new Entity<PowerLoaderComponent>(args.User, comp);
    EntityUid target = args.Target;
    ContainerSlot slot1;
    ContainerSlot container;
    if (this.CanDetachPopup(ref user, (EntityUid) ent, ent.Comp.AmmoContainerSlotId, false, out slot1) && slot1.ContainedEntity.HasValue)
    {
      container = slot1;
    }
    else
    {
      ContainerSlot slot2;
      if (!this.CanDetachPopup(ref user, (EntityUid) ent, ent.Comp.WeaponContainerSlotId, true, out slot2) || !slot2.ContainedEntity.HasValue)
        return;
      container = slot2;
    }
    this.StartPointDetach<DropshipWeaponPointComponent>(ent, container, (Entity<PowerLoaderComponent>) ((EntityUid) user, comp), target);
  }

  private void OnPointActivateInWorld(
    Entity<DropshipUtilityPointComponent> ent,
    ref ActivateInWorldEvent args)
  {
    this.TryStartPointDetach<DropshipUtilityPointComponent>(ent, ent.Comp.UtilitySlotId, ref args);
  }

  private void OnEngineActivateInWorld(
    Entity<DropshipEnginePointComponent> ent,
    ref ActivateInWorldEvent args)
  {
    this.TryStartPointDetach<DropshipEnginePointComponent>(ent, ent.Comp.ContainerId, ref args);
  }

  private void OnEngineActivateInWorld(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref ActivateInWorldEvent args)
  {
    this.TryStartPointDetach<DropshipElectronicSystemPointComponent>(ent, ent.Comp.ContainerId, ref args);
  }

  private void OnGrabbablePickupAttempt(
    Entity<PowerLoaderGrabbableComponent> ent,
    ref PickupAttemptEvent args)
  {
    if (args.Cancelled || this.HasComp<PowerLoaderComponent>(args.User))
      return;
    args.Cancel();
  }

  private void OnGrabbableAfterInteract(
    Entity<PowerLoaderGrabbableComponent> ent,
    ref AfterInteractEvent args)
  {
    this.TryDropLoaderHeld((Entity<PowerLoaderComponent>) args.User, args.ClickLocation, args.Used);
  }

  private void OnGetSlot(Entity<DropshipWeaponPointComponent> ent, ref GetAttachmentSlotEvent args)
  {
    Entity<PowerLoaderComponent> user = new Entity<PowerLoaderComponent>(this.GetEntity(args.User), (PowerLoaderComponent) null);
    EntityUid? entity;
    if (!this.TryGetEntity(args.Used, out entity))
      return;
    ContainerSlot slot;
    args.CanUse = !args.BeingAttached ? this.CanDetachPopup(ref user, (EntityUid) ent, ent.Comp.AmmoContainerSlotId, false, out slot) || this.CanDetachPopup(ref user, (EntityUid) ent, ent.Comp.WeaponContainerSlotId, false, out slot) : this.CanAttachPopup(ref user, ent, entity.Value, out slot);
    if (slot == null)
      return;
    args.SlotId = slot.ID;
  }

  private void OnGetSlot(Entity<DropshipUtilityPointComponent> ent, ref GetAttachmentSlotEvent args)
  {
    this.TryGetSlot((EntityUid) ent, ent.Comp.UtilitySlotId, ref args);
  }

  private void OnGetSlot(Entity<DropshipEnginePointComponent> ent, ref GetAttachmentSlotEvent args)
  {
    this.TryGetSlot((EntityUid) ent, ent.Comp.ContainerId, ref args);
  }

  private void OnGetSlot(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref GetAttachmentSlotEvent args)
  {
    this.TryGetSlot((EntityUid) ent, ent.Comp.ContainerId, ref args);
  }

  private void OnDropshipAttach(
    Entity<DropshipWeaponPointComponent> ent,
    ref DropshipAttachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out EntityUid _, out contained, out slot))
      return;
    this.InsertPoint(user, contained, slot);
    this.SyncAppearance((Entity<DropshipWeaponPointComponent>) ent.Owner);
  }

  private void OnDropshipAttach(
    Entity<DropshipUtilityPointComponent> ent,
    ref DropshipAttachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid container;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out container, out contained, out slot))
      return;
    DropshipUtilityComponent comp;
    if (this.TryComp<DropshipUtilityComponent>(contained, out comp))
      comp.AttachmentPoint = new EntityUid?(container);
    this.InsertPoint(user, contained, slot);
    this.SyncAppearance((EntityUid) ent, ent.Comp.UtilitySlotId);
  }

  private void OnDropshipAttach(
    Entity<DropshipEnginePointComponent> ent,
    ref DropshipAttachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out EntityUid _, out contained, out slot))
      return;
    this.InsertPoint(user, contained, slot);
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnDropshipAttach(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref DropshipAttachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out EntityUid _, out contained, out slot))
      return;
    this.InsertPoint(user, contained, slot);
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnDropshipDetach(
    Entity<DropshipWeaponPointComponent> ent,
    ref DropshipDetachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out EntityUid _, out contained, out slot))
      return;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) contained, slot);
    DropshipAmmoComponent comp;
    if (this.TryComp<DropshipAmmoComponent>(contained, out comp) && comp.Rounds < comp.RoundsPerShot)
    {
      this.QueueDel(new EntityUid?(contained));
      string message = this.Loc.GetString("rmc-power-loader-discard-empty", ("ammo", (object) contained));
      foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
        this._popup.PopupClient(message, new EntityUid?(entityUid), PopupType.Medium);
    }
    else
    {
      this.PickUp((Entity<PowerLoaderComponent>) ((EntityUid) user, user.Comp), contained);
      this.SyncHands((Entity<PowerLoaderComponent>) ((EntityUid) user, user.Comp));
    }
    this.SyncAppearance((Entity<DropshipWeaponPointComponent>) ent.Owner);
  }

  private void OnDropshipDetach(
    Entity<DropshipUtilityPointComponent> ent,
    ref DropshipDetachDoAfterEvent args)
  {
    this.DetachPoint(ref args);
    this.SyncAppearance((EntityUid) ent, ent.Comp.UtilitySlotId);
  }

  private void OnEngineDetach(
    Entity<DropshipEnginePointComponent> ent,
    ref DropshipDetachDoAfterEvent args)
  {
    this.DetachPoint(ref args);
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnEngineDetach(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref DropshipDetachDoAfterEvent args)
  {
    this.DetachPoint(ref args);
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnGrababbleShouldInteract(
    Entity<PowerLoaderGrabbableComponent> ent,
    ref CombatModeShouldHandInteractEvent args)
  {
    if (this.HasComp<PowerLoaderComponent>(args.User))
      return;
    args.Cancelled = true;
  }

  private void OnGrabbableBeforeRangedInteract(
    Entity<PowerLoaderGrabbableComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = true;
    Entity<PowerLoaderComponent> entity = new Entity<PowerLoaderComponent>(args.User, (PowerLoaderComponent) null);
    EntityUid used = args.Used;
    PowerLoaderInteractEvent args1 = new PowerLoaderInteractEvent(args.User, valueOrDefault, args.Used, this.GetBuckled(args.User).ToList<EntityUid>());
    this.RaiseLocalEvent<PowerLoaderInteractEvent>(used, ref args1);
    if (args1.Handled)
      return;
    GetAttachmentSlotEvent args2 = new GetAttachmentSlotEvent(this.GetNetEntity((EntityUid) entity), new NetEntity?(this.GetNetEntity(used)));
    this.RaiseLocalEvent<GetAttachmentSlotEvent>(valueOrDefault, args2);
    if (string.IsNullOrWhiteSpace(args2.SlotId))
      return;
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>(valueOrDefault, args2.SlotId);
    PowerLoaderAttachableComponent comp1;
    if (!args2.CanUse || !this.TryComp<PowerLoaderAttachableComponent>(used, out comp1) || !this._tag.HasAnyTag(valueOrDefault, comp1.AttachableTypes))
      return;
    DropshipAttachDoAfterEvent @event = new DropshipAttachDoAfterEvent(this.GetNetEntity(valueOrDefault), this.GetNetEntity(used), containerSlot.ID);
    PowerLoaderComponent comp2;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) entity, comp1.AttachDelay, (DoAfterEvent) @event, new EntityUid?(valueOrDefault), new EntityUid?(valueOrDefault), new EntityUid?(used))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      DistanceThreshold = new float?(2.5f)
    }) || !this.TryComp<PowerLoaderComponent>(args.User, out comp2))
      return;
    comp2.DoAfter = @event.DoAfter;
  }

  private void OnActivePilotPreventCollide(
    Entity<ActivePowerLoaderPilotComponent> ent,
    ref PreventCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnActivePilotStunned<T>(Entity<ActivePowerLoaderPilotComponent> ent, ref T args)
  {
    this.RemovePilot(ent);
  }

  private void OnActivePilotMobStateChanged(
    Entity<ActivePowerLoaderPilotComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Critical && args.NewMobState != MobState.Dead)
      return;
    this.OnActivePilotStunned<MobStateChangedEvent>(ent, ref args);
  }

  private void OnDropshipPartPowerLoaderInteract(
    Entity<DropshipFabricatorPrintableComponent> ent,
    ref PowerLoaderInteractEvent args)
  {
    DropshipFabricatorComponent comp1;
    if (args.Handled || !this.TryComp<DropshipFabricatorComponent>(args.Target, out comp1) || !this.HasComp<DropshipFabricatorPointsComponent>(comp1.Account))
      return;
    args.Handled = true;
    float num = 1f;
    MovementRelayTargetComponent comp2;
    if (this.TryComp<MovementRelayTargetComponent>(args.PowerLoader, out comp2))
      num = this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) comp2.Source, ent.Comp.RecycleSkill);
    TimeSpan delay = ent.Comp.Delay * (double) num;
    DropshipFabricatoreRecycleDoafterEvent @event = new DropshipFabricatoreRecycleDoafterEvent();
    PowerLoaderComponent comp3;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.PowerLoader, delay, (DoAfterEvent) @event, new EntityUid?(args.Target), new EntityUid?(args.Target), new EntityUid?(args.Used))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      DistanceThreshold = new float?(2.5f)
    }) || !this.TryComp<PowerLoaderComponent>(args.PowerLoader, out comp3))
      return;
    comp3.DoAfter = @event.DoAfter;
  }

  private bool CanAttachPopup(
    ref Entity<PowerLoaderComponent?> user,
    Entity<DropshipWeaponPointComponent> target,
    EntityUid used,
    [NotNullWhen(true)] out ContainerSlot? slot)
  {
    slot = (ContainerSlot) null;
    if (!this.Resolve<PowerLoaderComponent>((EntityUid) user, ref user.Comp, false))
      return false;
    string id;
    string message1;
    EntityUid? nullable;
    if (this.HasComp<DropshipWeaponComponent>(used))
    {
      id = target.Comp.WeaponContainerSlotId;
      message1 = this.Loc.GetString("rmc-power-loader-occupied-weapon");
    }
    else if (this.HasComp<DropshipAmmoComponent>(used))
    {
      nullable = this._transform.GetGrid((Entity<TransformComponent>) target.Owner);
      if (nullable.HasValue && this._dropship.IsInFlight((Entity<DropshipComponent>) nullable.GetValueOrDefault()))
        return false;
      id = target.Comp.AmmoContainerSlotId;
      message1 = this.Loc.GetString("rmc-power-loader-occupied-ammo");
      BaseContainer container;
      if (!this._container.TryGetContainer((EntityUid) target, target.Comp.WeaponContainerSlotId, out container) || container.ContainedEntities.Count == 0)
      {
        string message2 = this.Loc.GetString("rmc-power-loader-ammo-no-weapon");
        foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
          this._popup.PopupClient(message2, (EntityUid) target, new EntityUid?(entityUid), PopupType.SmallCaution);
        return false;
      }
    }
    else
    {
      if (!this.HasComp<RMCEquipmentDeployerComponent>(used))
        return false;
      id = target.Comp.WeaponContainerSlotId;
      message1 = this.Loc.GetString("rmc-power-loader-occupied-deployer");
    }
    slot = this._container.EnsureContainer<ContainerSlot>((EntityUid) target, id);
    nullable = slot.ContainedEntity;
    if (!nullable.HasValue)
      return true;
    foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
      this._popup.PopupClient(message1, (EntityUid) target, new EntityUid?(entityUid), PopupType.SmallCaution);
    slot = (ContainerSlot) null;
    return false;
  }

  private void CanAttachPopup(
    ref Entity<PowerLoaderComponent?> user,
    EntityUid target,
    string container,
    EntityUid used,
    [NotNullWhen(true)] out ContainerSlot? slot)
  {
    slot = (ContainerSlot) null;
    if (!this.Resolve<PowerLoaderComponent>((EntityUid) user, ref user.Comp, false) || !this.HasComp<DropshipUtilityComponent>(used) && !this.HasComp<DropshipEngineComponent>(used) && !this.HasComp<DropshipElectronicSystemComponent>(used))
      return;
    string id = container;
    string message = this.Loc.GetString("rmc-power-loader-occupied");
    slot = this._container.EnsureContainer<ContainerSlot>(target, id);
    if (!slot.ContainedEntity.HasValue)
      return;
    foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
      this._popup.PopupClient(message, target, new EntityUid?(entityUid), PopupType.SmallCaution);
    slot = (ContainerSlot) null;
  }

  private bool CanDetachPopup(
    ref Entity<PowerLoaderComponent?> user,
    EntityUid target,
    string containerId,
    bool doPopup,
    [NotNullWhen(true)] out ContainerSlot? slot)
  {
    slot = (ContainerSlot) null;
    if (!this.Resolve<PowerLoaderComponent>((EntityUid) user, ref user.Comp, false))
      return false;
    if (!this.HasFreeHands(user))
    {
      if (doPopup)
      {
        string message = this.Loc.GetString("rmc-power-loader-cant-grab-full", ("mech", (object) user.Owner));
        foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
          this._popup.PopupClient(message, target, new EntityUid?(entityUid), PopupType.SmallCaution);
      }
      return false;
    }
    BaseContainer container;
    if (this._container.TryGetContainer(target, containerId, out container) && container.ContainedEntities.Count > 0)
      slot = (ContainerSlot) container;
    if (slot != null)
      return true;
    if (doPopup)
    {
      foreach (EntityUid entityUid in this.GetBuckled((EntityUid) user))
        this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-nothing-attached"), (EntityUid) user, new EntityUid?(entityUid), PopupType.SmallCaution);
    }
    return false;
  }

  private bool HasFreeHands(Entity<PowerLoaderComponent?> user)
  {
    return this._hands.CountFreeHands((Entity<HandsComponent>) user.Owner) > 0;
  }

  private bool CanPickupPopup(
    Entity<PowerLoaderComponent> loader,
    Entity<PowerLoaderGrabbableComponent?> grabbable,
    out TimeSpan delay)
  {
    delay = TimeSpan.Zero;
    if (!this.Resolve<PowerLoaderGrabbableComponent>((EntityUid) grabbable, ref grabbable.Comp, false))
      return false;
    if (!this.HasFreeHands((Entity<PowerLoaderComponent>) ((EntityUid) loader, (PowerLoaderComponent) loader)))
    {
      string message = this.Loc.GetString("rmc-power-loader-cant-grab-full", ("mech", (object) loader));
      foreach (EntityUid uid in this.GetBuckled((EntityUid) loader))
        this._popup.PopupClient(message, uid, new EntityUid?(uid), PopupType.SmallCaution);
    }
    delay = grabbable.Comp.Delay;
    return true;
  }

  private IEnumerable<EntityUid> GetBuckled(EntityUid loader)
  {
    StrapComponent comp;
    if (this.TryComp<StrapComponent>(loader, out comp))
    {
      foreach (EntityUid buckledEntity in comp.BuckledEntities)
        yield return buckledEntity;
    }
  }

  private void SyncHands(Entity<PowerLoaderComponent> loader)
  {
    if (this._net.IsClient)
      return;
    Container container = this._container.EnsureContainer<Container>((EntityUid) loader, loader.Comp.VirtualContainerId);
    foreach (EntityUid entityUid1 in this.GetBuckled((EntityUid) loader))
    {
      foreach (EntityUid entityUid2 in this._hands.EnumerateHeld((Entity<HandsComponent>) entityUid1).ToArray<EntityUid>())
        this.Del(new EntityUid?(entityUid2));
      foreach (EntityUid entityUid3 in container.ContainedEntities.ToArray<EntityUid>())
      {
        this._virtualItem.DeleteInHandsMatching(entityUid1, entityUid3);
        this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid3, (BaseContainer) container);
        if (this._net.IsServer || this.IsClientSide(entityUid3))
          this.Del(new EntityUid?(entityUid3));
      }
    }
    List<(EntityUid?, EntProtoId, string, HandLocation)> valueTupleList = new List<(EntityUid?, EntProtoId, string, HandLocation)>();
    foreach (string enumerateHand in this._hands.EnumerateHands((Entity<HandsComponent>) loader.Owner))
    {
      Hand? hand;
      if (this._hands.TryGetHand((Entity<HandsComponent>) loader.Owner, enumerateHand, out hand))
      {
        EntityUid? held;
        if (!this._hands.TryGetHeldItem((Entity<HandsComponent>) loader.Owner, enumerateHand, out held))
        {
          EntProtoId entProtoId = hand.Value.Location == HandLocation.Right ? loader.Comp.VirtualRight : loader.Comp.VirtualLeft;
          valueTupleList.Add((new EntityUid?(), entProtoId, (string) null, hand.Value.Location));
        }
        else
        {
          EntProtoId entProtoId = PowerLoaderSystem.DefaultHandVisual;
          PowerLoaderGrabbableComponent component;
          if (this._powerLoaderGrabbableQuery.TryComp(held, out component))
            entProtoId = hand.Value.Location == HandLocation.Right ? component.VirtualRight : component.VirtualLeft;
          string str = this.Name(held.Value);
          valueTupleList.Add((held, entProtoId, str, hand.Value.Location));
        }
      }
    }
    foreach ((EntityUid? nullable, EntProtoId protoName, string str, HandLocation _) in valueTupleList)
    {
      EntityUid? uid;
      if (this.TrySpawnInContainer((string) protoName, (EntityUid) loader, loader.Comp.VirtualContainerId, out uid))
      {
        PowerLoaderVirtualItemComponent virtualItemComponent = this.EnsureComp<PowerLoaderVirtualItemComponent>(uid.Value);
        virtualItemComponent.Grabbed = nullable;
        this.Dirty(uid.Value, (IComponent) virtualItemComponent);
        if (str != null)
          this._metaData.SetEntityName(uid.Value, str);
        foreach (EntityUid entityUid in this.GetBuckled((EntityUid) loader))
        {
          EntityUid buckled = entityUid;
          string element;
          EntityUid? virtualItem;
          Hand? hand;
          if (this._hands.EnumerateHands((Entity<HandsComponent>) buckled).TryFirstOrDefault<string>((Func<string, bool>) (h => this._hands.TryGetHand((Entity<HandsComponent>) buckled, h, out hand) && hand.Value.Location == location), out element) && this._virtualItem.TrySpawnVirtualItemInHand(uid.Value, buckled, out virtualItem, empty: element))
            this.EnsureComp<UnremoveableComponent>(virtualItem.Value);
        }
      }
    }
  }

  public void TrySyncHands(Entity<PowerLoaderComponent?> loader)
  {
    if (!this.Resolve<PowerLoaderComponent>((EntityUid) loader, ref loader.Comp, false))
      return;
    this.SyncHands((Entity<PowerLoaderComponent>) ((EntityUid) loader, loader.Comp));
  }

  private void DeleteVirtuals(Entity<PowerLoaderComponent> loader, EntityUid user)
  {
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) this._container.EnsureContainer<Container>((EntityUid) loader, loader.Comp.VirtualContainerId).ContainedEntities)
      this._virtualItem.DeleteInHandsMatching(user, containedEntity);
  }

  private void RemoveLoader(Entity<PowerLoaderComponent> loader)
  {
    foreach (EntityUid entityUid in this.GetBuckled((EntityUid) loader))
    {
      if (!this.TerminatingOrDeleted(entityUid))
        this.DeleteVirtuals(loader, entityUid);
    }
  }

  private void PickUp(Entity<PowerLoaderComponent> loader, EntityUid target)
  {
    if (!this.CanPickupPopup(loader, (Entity<PowerLoaderGrabbableComponent>) target, out TimeSpan _))
      return;
    foreach (EntityUid ent in this.GetBuckled((EntityUid) loader))
    {
      string activeHand = this._hands.GetActiveHand((Entity<HandsComponent>) ent);
      string element;
      Hand? active;
      Hand? hand;
      if (activeHand != null && this._hands.TryGetHand((Entity<HandsComponent>) ent, activeHand, out active) && this._hands.EnumerateHands((Entity<HandsComponent>) loader.Owner).TryFirstOrDefault<string>((Func<string, bool>) (h => this._hands.TryGetHand((Entity<HandsComponent>) loader.Owner, h, out hand) && hand.Value.Location == active.Value.Location), out element))
      {
        this._hands.DoPickup((EntityUid) loader, element, target);
        this.SyncHands(loader);
        break;
      }
    }
  }

  public void SyncAppearance(Entity<DropshipWeaponPointComponent?> point)
  {
    if (!this.Resolve<DropshipWeaponPointComponent>((EntityUid) point, ref point.Comp, false))
      return;
    BaseContainer container1;
    if (!this._container.TryGetContainer((EntityUid) point, point.Comp.WeaponContainerSlotId, out container1) || container1.ContainedEntities.Count == 0)
    {
      this._appearance.SetData((EntityUid) point, (Enum) DropshipWeaponVisuals.Sprite, (object) "");
      this._appearance.SetData((EntityUid) point, (Enum) DropshipWeaponVisuals.State, (object) "");
    }
    else
    {
      bool flag = false;
      int num1 = 0;
      int num2 = 0;
      BaseContainer container2;
      if (this._container.TryGetContainer((EntityUid) point, point.Comp.AmmoContainerSlotId, out container2))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container2.ContainedEntities)
        {
          DropshipAmmoComponent comp;
          if (this.TryComp<DropshipAmmoComponent>(containedEntity, out comp))
          {
            num2 = comp.Rounds;
            num1 = comp.MaxRounds;
            if (comp.Rounds >= comp.RoundsPerShot)
              flag = true;
          }
        }
      }
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
      {
        SpriteSpecifier.Rsi rsi = (SpriteSpecifier.Rsi) null;
        DropshipWeaponComponent comp1;
        if (this.TryComp<DropshipWeaponComponent>(containedEntity, out comp1))
        {
          if (num2 > 0 & flag)
          {
            rsi = comp1.AmmoAttachedSprite;
            if (rsi != null && comp1.AmmoAttachedSprite != null && num2 != num1)
            {
              foreach (int ammoSpriteThreshold in comp1.AmmoSpriteThresholds)
              {
                if (ammoSpriteThreshold <= num2)
                {
                  rsi = new SpriteSpecifier.Rsi(rsi.RsiPath, $"{comp1.AmmoAttachedSprite.RsiState}_{ammoSpriteThreshold.ToString()}");
                  break;
                }
              }
            }
          }
          else
            rsi = num2 <= 0 ? comp1.WeaponAttachedSprite : comp1.AmmoEmptyAttachedSprite;
        }
        else
        {
          DropshipAttachedSpriteComponent comp2;
          if (this.TryComp<DropshipAttachedSpriteComponent>(containedEntity, out comp2))
            rsi = comp2.WeaponSlotSprite;
        }
        if (rsi != null)
        {
          this._appearance.SetData((EntityUid) point, (Enum) DropshipWeaponVisuals.Sprite, (object) rsi.RsiPath.ToString());
          this._appearance.SetData((EntityUid) point, (Enum) DropshipWeaponVisuals.State, (object) rsi.RsiState);
        }
      }
    }
  }

  private void SyncAppearance(EntityUid point, string container)
  {
    BaseContainer container1;
    if (!this._container.TryGetContainer(point, container, out container1) || container1.ContainedEntities.Count == 0)
    {
      this._appearance.SetData(point, (Enum) DropshipUtilityVisuals.Sprite, (object) "");
      this._appearance.SetData(point, (Enum) DropshipUtilityVisuals.State, (object) "");
    }
    else
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
      {
        DropshipAttachedSpriteComponent comp;
        if (this.TryComp<DropshipAttachedSpriteComponent>(containedEntity, out comp))
        {
          SpriteSpecifier.Rsi sprite = comp.Sprite;
          if (sprite != null)
          {
            this._appearance.SetData(point, (Enum) DropshipUtilityVisuals.Sprite, (object) sprite.RsiPath.ToString());
            this._appearance.SetData(point, (Enum) DropshipUtilityVisuals.State, (object) sprite.RsiState);
            break;
          }
        }
      }
    }
  }

  private void RemovePilot(Entity<ActivePowerLoaderPilotComponent> active)
  {
    this._buckle.Unbuckle((Entity<BuckleComponent>) active.Owner, new EntityUid?());
    this.RemCompDeferred<ActivePowerLoaderPilotComponent>((EntityUid) active);
  }

  private bool TryGetPointContainer(
    DropshipDoAfterEvent args,
    out Entity<PowerLoaderComponent> user,
    out EntityUid container,
    out EntityUid contained,
    [NotNullWhen(true)] out BaseContainer? slot)
  {
    user = new Entity<PowerLoaderComponent>();
    container = new EntityUid();
    contained = new EntityUid();
    slot = (BaseContainer) null;
    PowerLoaderComponent comp;
    if (args.Cancelled || args.Handled || !args.Target.HasValue || !this.TryComp<PowerLoaderComponent>(args.User, out comp))
      return false;
    args.Handled = true;
    user = new Entity<PowerLoaderComponent>(args.User, comp);
    container = this.GetEntity(args.Container);
    contained = this.GetEntity(args.Contained);
    slot = this._container.GetContainer(container, args.Slot);
    return true;
  }

  private void InsertPoint(
    Entity<PowerLoaderComponent> user,
    EntityUid contained,
    BaseContainer slot)
  {
    if (slot.ContainedEntities.Count > 0)
      return;
    DropShipAttachmentInsertedEvent args = new DropShipAttachmentInsertedEvent(contained);
    this.RaiseLocalEvent<DropShipAttachmentInsertedEvent>(slot.Owner, ref args);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) contained, slot);
    this.SyncHands((Entity<PowerLoaderComponent>) ((EntityUid) user, user.Comp));
  }

  private void StartPointDetach<T>(
    Entity<T> ent,
    ContainerSlot container,
    Entity<PowerLoaderComponent> user,
    EntityUid target)
    where T : IComponent?
  {
    PowerLoaderDetachableComponent comp;
    if (!this.TryComp<PowerLoaderDetachableComponent>(container.ContainedEntity, out comp))
      return;
    EntityUid uid = container.ContainedEntity.Value;
    DropshipDetachDoAfterEvent @event = new DropshipDetachDoAfterEvent(this.GetNetEntity((EntityUid) ent), this.GetNetEntity(uid), container.ID);
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) user, comp.DetachDelay, (DoAfterEvent) @event, new EntityUid?(target), new EntityUid?(target))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      DistanceThreshold = new float?(2.5f)
    }))
      return;
    user.Comp.DoAfter = @event.DoAfter;
  }

  private void TryStartPointDetach<T>(
    Entity<T> ent,
    string container,
    ref ActivateInWorldEvent args)
    where T : IComponent?
  {
    PowerLoaderComponent comp;
    if (!this.TryComp<PowerLoaderComponent>(args.User, out comp))
      return;
    args.Handled = true;
    Entity<PowerLoaderComponent> user = new Entity<PowerLoaderComponent>(args.User, comp);
    EntityUid target = args.Target;
    ContainerSlot slot;
    if (!this.CanDetachPopup(ref user, (EntityUid) ent, container, true, out slot) || !slot.ContainedEntity.HasValue)
      return;
    this.StartPointDetach<T>(ent, slot, (Entity<PowerLoaderComponent>) ((EntityUid) user, comp), target);
  }

  private void DetachPoint(ref DropshipDetachDoAfterEvent args)
  {
    Entity<PowerLoaderComponent> user;
    EntityUid contained;
    BaseContainer slot;
    if (!this.TryGetPointContainer((DropshipDoAfterEvent) args, out user, out EntityUid _, out contained, out slot))
      return;
    DropshipUtilityComponent comp;
    if (this.TryComp<DropshipUtilityComponent>(contained, out comp))
      comp.AttachmentPoint = new EntityUid?();
    DropShipAttachmentDetachedEvent args1 = new DropShipAttachmentDetachedEvent(contained);
    this.RaiseLocalEvent<DropShipAttachmentDetachedEvent>(slot.Owner, ref args1);
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) contained, slot);
    this.PickUp((Entity<PowerLoaderComponent>) ((EntityUid) user, user.Comp), contained);
    this.SyncHands((Entity<PowerLoaderComponent>) ((EntityUid) user, user.Comp));
  }

  private void TryGetSlot(EntityUid ent, string container, ref GetAttachmentSlotEvent args)
  {
    Entity<PowerLoaderComponent> user = new Entity<PowerLoaderComponent>(this.GetEntity(args.User), (PowerLoaderComponent) null);
    EntityUid? entity;
    if (!this.TryGetEntity(args.Used, out entity))
      return;
    ContainerSlot slot;
    if (args.BeingAttached)
      this.CanAttachPopup(ref user, ent, container, entity.Value, out slot);
    else
      this.CanDetachPopup(ref user, ent, container, true, out slot);
    if (slot == null)
      return;
    args.SlotId = slot.ID;
  }

  private void OnWeaponPointContainerChanged(
    Entity<DropshipWeaponPointComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this.SyncAppearance((Entity<DropshipWeaponPointComponent>) ent.Owner);
  }

  private void OnUtilityPointContainerChanged(
    Entity<DropshipUtilityPointComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!(args.Container.ID == ent.Comp.UtilitySlotId))
      return;
    this.SyncAppearance((EntityUid) ent, ent.Comp.UtilitySlotId);
  }

  private void OnEnginePointContainerChanged(
    Entity<DropshipEnginePointComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!(args.Container.ID == ent.Comp.ContainerId))
      return;
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnElectronicPointContainerChanged(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!(args.Container.ID == ent.Comp.ContainerId))
      return;
    this.SyncAppearance((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnPowerLoaderPilotCatchAttempt(
    Entity<ActivePowerLoaderPilotComponent> pilot,
    ref CatchAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void PowerLoaderBeforeMelted(
    Entity<PowerLoaderComponent> loader,
    ref BeforeMeltedEvent args)
  {
    foreach (EntityUid entity in this._hands.EnumerateHeld((Entity<HandsComponent>) loader.Owner).ToList<EntityUid>())
      this._hands.TryDrop((Entity<HandsComponent>) loader.Owner, entity);
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>((EntityUid) loader, out comp))
      return;
    foreach (EntityUid buckle in comp.BuckledEntities.ToArray<EntityUid>())
      this._buckle.Unbuckle((Entity<BuckleComponent>) buckle, new EntityUid?());
  }

  private bool TryDropLoaderHeld(
    Entity<PowerLoaderComponent?> loader,
    EntityCoordinates clickLocation,
    EntityUid item)
  {
    if (!this.Resolve<PowerLoaderComponent>((EntityUid) loader, ref loader.Comp, false) || !this._hands.IsHolding((Entity<HandsComponent>) loader.Owner, new EntityUid?(item)))
      return false;
    EntityCoordinates coordinates = loader.Owner.ToCoordinates();
    EntityCoordinates grid = this._transform.GetMoverCoordinates(clickLocation).SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
    float distance;
    if (!coordinates.TryDistance((IEntityManager) this.EntityManager, grid, out distance))
      return false;
    if ((double) distance < 0.5)
    {
      string message = this.Loc.GetString("rmc-power-loader-too-close");
      foreach (EntityUid entityUid in this.GetBuckled((EntityUid) loader))
        this._popup.PopupClient(message, (EntityUid) loader, new EntityUid?(entityUid), PopupType.SmallCaution);
      return true;
    }
    if ((double) distance > 1.5)
    {
      string message = this.Loc.GetString("rmc-power-loader-too-far");
      foreach (EntityUid entityUid in this.GetBuckled((EntityUid) loader))
        this._popup.PopupClient(message, (EntityUid) loader, new EntityUid?(entityUid), PopupType.SmallCaution);
      return true;
    }
    if (this._rmcMap.IsTileBlocked(grid, CollisionGroup.AirlockLayer | CollisionGroup.Impassable) || this._rmcMap.TileHasStructure(grid))
    {
      string message = this.Loc.GetString("rmc-power-loader-cant-drop-occupied", ("drop", (object) item));
      foreach (EntityUid entityUid in this.GetBuckled((EntityUid) loader))
        this._popup.PopupClient(message, (EntityUid) loader, new EntityUid?(entityUid), PopupType.SmallCaution);
      return true;
    }
    if (this._hands.TryDrop((Entity<HandsComponent>) loader.Owner, item, new EntityCoordinates?(grid), false))
    {
      if (this._powerLoaderGrabbableQuery.HasComp(item))
        this._transform.AnchorEntity((Entity<TransformComponent>) (item, this.Transform(item)));
      this.SyncHands((Entity<PowerLoaderComponent>) ((EntityUid) loader, loader.Comp));
    }
    return true;
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActivePowerLoaderPilotComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActivePowerLoaderPilotComponent>();
    EntityUid uid;
    ActivePowerLoaderPilotComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      BuckleComponent comp;
      if (!this.TryComp<BuckleComponent>(uid, out comp) || !this.HasComp<PowerLoaderComponent>(comp.BuckledTo))
        this.RemovePilot((Entity<ActivePowerLoaderPilotComponent>) (uid, comp1));
    }
  }
}
