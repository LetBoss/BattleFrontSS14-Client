// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Devour.XenoDevourSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Attachable;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Vents;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Devour;

public sealed class XenoDevourSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private SharedMeleeWeaponSystem _meleeWeapon;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  private Robust.Shared.GameObjects.EntityQuery<DevouredComponent> _devouredQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoDevourComponent> _xenoDevourQuery;

  public override void Initialize()
  {
    this._devouredQuery = this.GetEntityQuery<DevouredComponent>();
    this._xenoDevourQuery = this.GetEntityQuery<XenoDevourComponent>();
    this.SubscribeLocalEvent<ActionBlockIfDevouredComponent, RMCActionUseAttemptEvent>(new EntityEventRefHandler<ActionBlockIfDevouredComponent, RMCActionUseAttemptEvent>(this.OnDevouredActionUseAttempt));
    this.SubscribeLocalEvent<DevourableComponent, CanDropDraggedEvent>(new EntityEventRefHandler<DevourableComponent, CanDropDraggedEvent>(this.OnDevourableCanDropDragged));
    this.SubscribeLocalEvent<DevourableComponent, DragDropDraggedEvent>(new EntityEventRefHandler<DevourableComponent, DragDropDraggedEvent>(this.OnDevourableDragDropDragged));
    this.SubscribeLocalEvent<DevourableComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<DevourableComponent, BeforeRangedInteractEvent>(this.OnDevourableBeforeRangedInteract));
    this.SubscribeLocalEvent<DevourableComponent, ShouldHandleVirtualItemInteractEvent>(new EntityEventRefHandler<DevourableComponent, ShouldHandleVirtualItemInteractEvent>(this.OnDevourableShouldHandle));
    this.SubscribeLocalEvent<DevouredComponent, ComponentStartup>(new EntityEventRefHandler<DevouredComponent, ComponentStartup>(this.OnDevouredStartup));
    this.SubscribeLocalEvent<DevouredComponent, ComponentRemove>(new EntityEventRefHandler<DevouredComponent, ComponentRemove>(this.OnDevouredRemove));
    this.SubscribeLocalEvent<DevouredComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<DevouredComponent, EntGotRemovedFromContainerMessage>(this.OnDevouredRemovedFromContainer));
    this.SubscribeLocalEvent<DevouredComponent, InteractionAttemptEvent>(new EntityEventRefHandler<DevouredComponent, InteractionAttemptEvent>(this.OnDevouredInteractionAttempt));
    this.SubscribeLocalEvent<DevouredComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<DevouredComponent, UpdateCanMoveEvent>(this.OnDevouredAttempt<UpdateCanMoveEvent>));
    this.SubscribeLocalEvent<DevouredComponent, ThrowAttemptEvent>(new EntityEventRefHandler<DevouredComponent, ThrowAttemptEvent>(this.OnDevouredAttempt<ThrowAttemptEvent>));
    this.SubscribeLocalEvent<DevouredComponent, DropAttemptEvent>(new EntityEventRefHandler<DevouredComponent, DropAttemptEvent>(this.OnDevouredAttempt<DropAttemptEvent>));
    this.SubscribeLocalEvent<DevouredComponent, UseAttemptEvent>(new EntityEventRefHandler<DevouredComponent, UseAttemptEvent>(this.OnUseAttempt));
    this.SubscribeLocalEvent<DevouredComponent, PickupAttemptEvent>(new EntityEventRefHandler<DevouredComponent, PickupAttemptEvent>(this.OnDevouredPickupAttempt));
    this.SubscribeLocalEvent<DevouredComponent, IsEquippingAttemptEvent>(new EntityEventRefHandler<DevouredComponent, IsEquippingAttemptEvent>(this.OnDevouredIsEquippingAttempt));
    this.SubscribeLocalEvent<DevouredComponent, IsUnequippingAttemptEvent>(new EntityEventRefHandler<DevouredComponent, IsUnequippingAttemptEvent>(this.OnDevouredIsUnequippingAttempt));
    this.SubscribeLocalEvent<DevouredComponent, RMCCombatModeInteractOverrideUserEvent>(new EntityEventRefHandler<DevouredComponent, RMCCombatModeInteractOverrideUserEvent>(this.OnDevouredTryAttack));
    this.SubscribeLocalEvent<DevouredComponent, ShotAttemptedEvent>(new EntityEventRefHandler<DevouredComponent, ShotAttemptedEvent>(this.OnDevouredShotAttempted));
    this.SubscribeLocalEvent<DevouredComponent, MoveInputEvent>(new EntityEventRefHandler<DevouredComponent, MoveInputEvent>(this.OnDevouredMoveInput));
    this.SubscribeLocalEvent<UsableWhileDevouredComponent, MeleeHitEvent>(new EntityEventRefHandler<UsableWhileDevouredComponent, MeleeHitEvent>(this.UsuableByDevouredMeleeHit));
    this.SubscribeLocalEvent<XenoDevourComponent, CanDropTargetEvent>(new EntityEventRefHandler<XenoDevourComponent, CanDropTargetEvent>(this.OnXenoCanDropTarget));
    this.SubscribeLocalEvent<XenoDevourComponent, ActivateInWorldEvent>(new EntityEventRefHandler<XenoDevourComponent, ActivateInWorldEvent>(this.OnXenoActivate));
    this.SubscribeLocalEvent<XenoDevourComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>(new EntityEventRefHandler<XenoDevourComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>(this.OnXenoDevourDoAfterAttempt));
    this.SubscribeLocalEvent<XenoDevourComponent, XenoDevourDoAfterEvent>(new EntityEventRefHandler<XenoDevourComponent, XenoDevourDoAfterEvent>(this.OnXenoDevourDoAfter));
    this.SubscribeLocalEvent<XenoDevourComponent, XenoRegurgitateActionEvent>(new EntityEventRefHandler<XenoDevourComponent, XenoRegurgitateActionEvent>(this.OnXenoRegurgitateAction));
    this.SubscribeLocalEvent<XenoDevourComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoDevourComponent, EntityTerminatingEvent>(this.OnXenoTerminating));
    this.SubscribeLocalEvent<XenoDevourComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoDevourComponent, MobStateChangedEvent>(this.OnXenoMobStateChanged));
    this.SubscribeLocalEvent<XenoDevourComponent, RMCCombatModeInteractOverrideUserEvent>(new EntityEventRefHandler<XenoDevourComponent, RMCCombatModeInteractOverrideUserEvent>(this.OnXenoCombatModeInteract));
    this.SubscribeLocalEvent<XenoDevourComponent, InteractUsingEvent>(new EntityEventRefHandler<XenoDevourComponent, InteractUsingEvent>(this.OnXenoDevouredInteractWith));
    this.SubscribeLocalEvent<XenoDevourComponent, VentEnterAttemptEvent>(new EntityEventRefHandler<XenoDevourComponent, VentEnterAttemptEvent>(this.OnXenoDevouredVentAttempt));
    this.SubscribeLocalEvent<UsableWhileDevouredComponent, CMGetArmorPiercingEvent>(new EntityEventRefHandler<UsableWhileDevouredComponent, CMGetArmorPiercingEvent>(this.OnUsableWhileDevouredGetArmorPiercing));
  }

  private void OnDevouredActionUseAttempt(
    Entity<ActionBlockIfDevouredComponent> ent,
    ref RMCActionUseAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid user = args.User;
    if (!this.HasComp<DevouredComponent>(user))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("comp-climbable-cant-interact"), user, new EntityUid?(user), PopupType.SmallCaution);
  }

  private void OnDevourableShouldHandle(
    Entity<DevourableComponent> ent,
    ref ShouldHandleVirtualItemInteractEvent args)
  {
    if (!this.HasComp<XenoDevourComponent>(args.Event.User))
      return;
    EntityUid user = args.Event.User;
    EntityUid? target = args.Event.Target;
    if ((target.HasValue ? (user == target.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    args.Handle = true;
  }

  private void OnDevourableCanDropDragged(
    Entity<DevourableComponent> devourable,
    ref CanDropDraggedEvent args)
  {
    if (!this.HasComp<XenoDevourComponent>(args.User))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnDevourableDragDropDragged(
    Entity<DevourableComponent> devourable,
    ref DragDropDraggedEvent args)
  {
    if (args.User != args.Target || !this.StartDevour(args.User, (EntityUid) devourable))
      return;
    args.Handled = true;
  }

  private void OnDevourableBeforeRangedInteract(
    Entity<DevourableComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    EntityUid user = args.User;
    EntityUid? target = args.Target;
    if ((target.HasValue ? (user != target.GetValueOrDefault() ? 1 : 0) : 1) != 0 || !this.StartDevourPulled(args.User))
      return;
    args.Handled = true;
  }

  private void OnDevouredStartup(Entity<DevouredComponent> devoured, ref ComponentStartup args)
  {
    this._blocker.UpdateCanMove((EntityUid) devoured);
  }

  private void OnDevouredRemove(Entity<DevouredComponent> devoured, ref ComponentRemove args)
  {
    this._blocker.UpdateCanMove((EntityUid) devoured);
    BaseContainer container;
    XenoDevourComponent comp;
    if (this._timing.ApplyingState || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) devoured, (TransformComponent) null), out container) || !this.TryComp<XenoDevourComponent>(container.Owner, out comp) || !(container.ID != comp.DevourContainerId))
      return;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) devoured.Owner, container);
  }

  private void OnDevouredRemovedFromContainer(
    Entity<DevouredComponent> devoured,
    ref EntGotRemovedFromContainerMessage args)
  {
    if (this._timing.ApplyingState)
      return;
    this.RemCompDeferred<DevouredComponent>((EntityUid) devoured);
  }

  private void OnDevouredInteractionAttempt(
    Entity<DevouredComponent> ent,
    ref InteractionAttemptEvent args)
  {
    if (!args.Target.HasValue || this.HasComp<UsableWhileDevouredComponent>(args.Target))
      return;
    BaseContainer container;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ent.Owner, out container) && this.HasComp<XenoDevourComponent>(container.Owner))
    {
      EntityUid? target = args.Target;
      EntityUid owner = container.Owner;
      if ((target.HasValue ? (target.GetValueOrDefault() != owner ? 1 : 0) : 1) == 0)
        return;
    }
    args.Cancelled = true;
  }

  private void OnXenoDevouredInteractWith(
    Entity<XenoDevourComponent> ent,
    ref InteractUsingEvent args)
  {
    BaseContainer container;
    DevouredComponent comp;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.DevourContainerId, out container) || !container.Contains(args.User) || !this.TryComp<DevouredComponent>(args.User, out comp))
      return;
    args.Handled = true;
    this.DevouredHandleBreakout((Entity<DevouredComponent>) (args.User, comp));
  }

  private void OnDevouredAttempt<T>(Entity<DevouredComponent> devoured, ref T args) where T : CancellableEntityEventArgs
  {
    args.Cancel();
  }

  private void OnUseAttempt(Entity<DevouredComponent> ent, ref UseAttemptEvent args)
  {
    if (this.HasComp<UsableWhileDevouredComponent>(args.Used))
      return;
    args.Cancel();
  }

  private void OnDevouredPickupAttempt(Entity<DevouredComponent> ent, ref PickupAttemptEvent args)
  {
    if (this.HasComp<UsableWhileDevouredComponent>(args.Item))
      return;
    args.Cancel();
  }

  private void OnDevouredIsEquippingAttempt(
    Entity<DevouredComponent> devoured,
    ref IsEquippingAttemptEvent args)
  {
    if (this.HasComp<UsableWhileDevouredComponent>(args.Equipment))
      return;
    args.Cancel();
  }

  private void OnDevouredIsUnequippingAttempt(
    Entity<DevouredComponent> devoured,
    ref IsUnequippingAttemptEvent args)
  {
    UsableWhileDevouredComponent comp;
    if (this.TryComp<UsableWhileDevouredComponent>(args.Equipment, out comp) && comp.CanUnequip)
      return;
    args.Cancel();
  }

  private void OnDevouredShotAttempted(
    Entity<DevouredComponent> devoured,
    ref ShotAttemptedEvent args)
  {
    if (this.HasComp<GunUsableWhileDevouredComponent>((EntityUid) args.Used))
      return;
    args.Cancel();
  }

  private void OnDevouredMoveInput(Entity<DevouredComponent> devoured, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    this.DevouredHandleBreakout(devoured);
  }

  private void OnDevouredTryAttack(
    Entity<DevouredComponent> devoured,
    ref RMCCombatModeInteractOverrideUserEvent args)
  {
    args.Handled = true;
    args.CanInteract = true;
  }

  private void UsuableByDevouredMeleeHit(
    Entity<UsableWhileDevouredComponent> weapon,
    ref MeleeHitEvent args)
  {
    DevouredComponent comp;
    if (!this.TryComp<DevouredComponent>(args.User, out comp))
      return;
    args.Handled = true;
    this.DevouredHandleBreakout((Entity<DevouredComponent>) (args.User, comp));
  }

  private void DevouredHandleBreakout(Entity<DevouredComponent> devoured)
  {
    if (!this._timing.IsFirstTimePredicted || this._timing.CurTime < devoured.Comp.NextDevouredAttackTimeAllowed || this.HasComp<StunnedComponent>((EntityUid) devoured) || !this._mobState.IsAlive((EntityUid) devoured))
      return;
    devoured.Comp.NextDevouredAttackTimeAllowed = this._timing.CurTime + devoured.Comp.TimeBetweenStruggles;
    this.Dirty<DevouredComponent>(devoured);
    BaseContainer container;
    XenoDevourComponent comp1;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) devoured, (TransformComponent) null), out container) || !this.TryComp<XenoDevourComponent>(container.Owner, out comp1) || container.ID != comp1.DevourContainerId)
      return;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) devoured.Owner);
    MeleeWeaponComponent comp2;
    if (!activeItem.HasValue || !this.TryComp<UsableWhileDevouredComponent>(activeItem, out UsableWhileDevouredComponent _) || !this.TryComp<MeleeWeaponComponent>(activeItem, out comp2))
      return;
    DamageSpecifier damage = new DamageSpecifier(this._meleeWeapon.GetDamage(activeItem.Value, (EntityUid) devoured));
    BonusMeleeDamageComponent comp3;
    if (this.TryComp<BonusMeleeDamageComponent>(activeItem.Value, out comp3) && comp3.BonusDamage != null)
      damage += comp3.BonusDamage;
    TransformChildrenEnumerator childEnumerator = this.Transform(activeItem.Value).ChildEnumerator;
    EntityUid child;
    while (childEnumerator.MoveNext(out child))
    {
      AttachableWeaponMeleeModsComponent comp4;
      if (this.TryComp<AttachableWeaponMeleeModsComponent>(child, out comp4))
      {
        foreach (AttachableWeaponMeleeModifierSet modifier in comp4.Modifiers)
        {
          if (modifier.BonusDamage != null)
            damage += modifier.BonusDamage;
        }
      }
    }
    comp2.NextAttack = devoured.Comp.NextDevouredAttackTimeAllowed;
    this.Dirty(activeItem.Value, (IComponent) comp2);
    DamageSpecifier damageSpecifier = this._damage.TryChangeDamage(new EntityUid?(container.Owner), damage, true, false, origin: new EntityUid?((EntityUid) devoured), tool: activeItem);
    this._audio.PlayPredicted(comp2.HitSound, container.Owner.ToCoordinates(), new EntityUid?((EntityUid) devoured));
    FixedPoint2? total = damageSpecifier?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) == 0)
      return;
    Filter filter1 = Filter.Pvs(container.Owner, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == devoured.Owner));
    SharedColorFlashEffectSystem colorFlash = this._colorFlash;
    Color red = Color.Red;
    List<EntityUid> entities = new List<EntityUid>();
    entities.Add(container.Owner);
    Filter filter2 = filter1;
    colorFlash.RaiseEffect(red, entities, filter2);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(52, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) devoured)), "actor", "ToPrettyString(devoured)");
    logStringHandler.AppendLiteral(" attacked while devoured by ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) container.Owner), "subject", "ToPrettyString(container.Owner)");
    logStringHandler.AppendLiteral(" with ");
    logStringHandler.AppendFormatted<EntityUid?>(activeItem, "weapon");
    logStringHandler.AppendLiteral(" and dealt ");
    logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier.GetTotal(), "damage", "damage.GetTotal()");
    logStringHandler.AppendLiteral(" damage");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref local);
  }

  private void OnXenoCanDropTarget(Entity<XenoDevourComponent> xeno, ref CanDropTargetEvent args)
  {
    if (args.Handled || !this.HasComp<DevourableComponent>(args.Dragged) || !(xeno.Owner == args.User))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnXenoActivate(Entity<XenoDevourComponent> xeno, ref ActivateInWorldEvent args)
  {
    if (args.User != args.Target || !this.StartDevourPulled(args.User))
      return;
    args.Handled = true;
  }

  private void OnXenoDevourDoAfterAttempt(
    Entity<XenoDevourComponent> ent,
    ref DoAfterAttemptEvent<XenoDevourDoAfterEvent> args)
  {
    EntityUid? target = args.DoAfter.Args.Target;
    if (target.HasValue)
    {
      EntityUid valueOrDefault = target.GetValueOrDefault();
      if (this.CanDevour((EntityUid) ent, valueOrDefault, out XenoDevourComponent _, true))
        return;
    }
    args.Cancel();
  }

  private void OnXenoDevourDoAfter(
    Entity<XenoDevourComponent> xeno,
    ref XenoDevourDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    if (!this.CanDevour((EntityUid) xeno, valueOrDefault1, out XenoDevourComponent _, true))
      return;
    args.Handled = true;
    XenoTargetDevouredAttemptEvent args1 = new XenoTargetDevouredAttemptEvent();
    this.RaiseLocalEvent<XenoTargetDevouredAttemptEvent>(valueOrDefault1, ref args1);
    if (args1.Cancelled)
      return;
    EntityUid entityUid = Identity.Entity(valueOrDefault1, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) xeno));
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) xeno, xeno.Comp.DevourContainerId);
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault1, (BaseContainer) container))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-failed", ("target", (object) entityUid)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      DevouredComponent devouredComponent = this.EnsureComp<DevouredComponent>(valueOrDefault1);
      devouredComponent.WarnAt = this._timing.CurTime + xeno.Comp.WarnAfter;
      devouredComponent.RegurgitateAt = this._timing.CurTime + xeno.Comp.RegurgitateAfter;
      devouredComponent.NextDevouredAttackTimeAllowed = TimeSpan.Zero;
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-self", ("target", (object) entityUid)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
      this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-target", ("user", (object) xeno.Owner)), (EntityUid) xeno, valueOrDefault1, PopupType.MediumCaution);
      foreach (ICommonSession recipient1 in Filter.PvsExcept((EntityUid) xeno).RemovePlayerByAttachedEntity(valueOrDefault1).Recipients)
      {
        nullable = recipient1.AttachedEntity;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
          SharedPopupSystem popup = this._popup;
          ILocalizationManager loc = this.Loc;
          EntityUid uid1 = (EntityUid) xeno;
          EntityManager entityManager = this.EntityManager;
          nullable = new EntityUid?();
          EntityUid? viewer = nullable;
          (string, object) valueTuple1 = ("user", (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
          (string, object) valueTuple2 = ("target", (object) entityUid);
          string message = loc.GetString("cm-xeno-devour-observer", valueTuple1, valueTuple2);
          EntityUid uid2 = (EntityUid) xeno;
          EntityUid recipient2 = valueOrDefault2;
          popup.PopupEntity(message, uid2, recipient2, PopupType.MediumCaution);
        }
      }
      XenoDevouredEvent args2 = new XenoDevouredEvent(valueOrDefault1, xeno.Owner);
      this.RaiseLocalEvent<XenoDevouredEvent>(valueOrDefault1, ref args2, true);
    }
  }

  private void OnXenoRegurgitateAction(
    Entity<XenoDevourComponent> xeno,
    ref XenoRegurgitateActionEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) xeno, xeno.Comp.DevourContainerId, out container) || container.ContainedEntities.Count == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-none-devoured"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
    else
    {
      args.Handled = true;
      List<EntityUid> entityUidList = this._container.EmptyContainer(container);
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-hurl-out"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
      this._audio.PlayPredicted(xeno.Comp.RegurgitateSound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      foreach (EntityUid uid in entityUidList)
      {
        RegurgitateEvent args1 = new RegurgitateEvent(this.GetNetEntity(xeno.Owner), this.GetNetEntity(uid));
        this.RaiseLocalEvent<RegurgitateEvent>((EntityUid) xeno, args1);
        this._stun.TryStun(uid, xeno.Comp.RegurgitationStun, true);
      }
    }
  }

  private void OnXenoTerminating(Entity<XenoDevourComponent> xeno, ref EntityTerminatingEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this.RegurgitateAll(xeno);
  }

  private void OnXenoMobStateChanged(
    Entity<XenoDevourComponent> xeno,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    this.RegurgitateAll(xeno);
  }

  private void OnXenoCombatModeInteract(
    Entity<XenoDevourComponent> ent,
    ref RMCCombatModeInteractOverrideUserEvent args)
  {
    EntityUid owner = ent.Owner;
    EntityUid? target = args.Target;
    if ((target.HasValue ? (owner != target.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      return;
    EntityUid? pulling = (EntityUid?) this.CompOrNull<PullerComponent>((EntityUid) ent)?.Pulling;
    if (!pulling.HasValue || !this.HasComp<DevourableComponent>(pulling.GetValueOrDefault()))
      return;
    args.Handled = true;
  }

  private void OnXenoDevouredVentAttempt(
    Entity<XenoDevourComponent> ent,
    ref VentEnterAttemptEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.DevourContainerId, out container))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      if (!this.HasComp<InfectStopOnDeathComponent>(containedEntity))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-vent-crawling-devoured"), (EntityUid) ent, new EntityUid?((EntityUid) ent), PopupType.SmallCaution);
        args.Cancel();
        break;
      }
    }
  }

  private void OnUsableWhileDevouredGetArmorPiercing(
    Entity<UsableWhileDevouredComponent> ent,
    ref CMGetArmorPiercingEvent args)
  {
    if (!this.IsHeldByDevoured((EntityUid) ent))
      return;
    args.Piercing += 100;
  }

  private bool IsHeldByDevoured(EntityUid item)
  {
    BaseContainer container1;
    BaseContainer container2;
    XenoDevourComponent component;
    return this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (item, (TransformComponent) null), out container1) && this._devouredQuery.HasComp(container1.Owner) && this._hands.IsHolding((Entity<HandsComponent>) container1.Owner, new EntityUid?(item)) && this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (container1.Owner, (TransformComponent) null), out container2) && this._xenoDevourQuery.TryComp(container2.Owner, out component) && container2.ID == component.DevourContainerId;
  }

  private bool CanDevour(
    EntityUid xeno,
    EntityUid victim,
    [NotNullWhen(true)] out XenoDevourComponent? devour,
    bool popup = false)
  {
    devour = (XenoDevourComponent) null;
    if (xeno == victim || !this.TryComp<XenoDevourComponent>(xeno, out devour) || this.HasComp<DevouredComponent>(victim) || !this.HasComp<DevourableComponent>(victim))
      return false;
    if (this._mobState.IsIncapacitated(xeno) || this.HasComp<XenoNestedComponent>(victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-failed-cant-now"), victim, new EntityUid?(xeno));
      return false;
    }
    if (this.HasComp<SynthComponent>(victim) || this.HasComp<RMCTrainingDummyComponent>(victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-fake-host"), victim, new EntityUid?(xeno));
      return false;
    }
    if (this.HasComp<XenoComponent>(victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-success"), victim, new EntityUid?(xeno));
      return false;
    }
    if (this._mobState.IsDead(victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-failed-target-roting", ("target", (object) victim)), victim, new EntityUid?(xeno));
      return false;
    }
    BaseContainer container;
    if (this._container.TryGetContainer(xeno, devour.DevourContainerId, out container) && container.ContainedEntities.Count > 0)
    {
      devour = (XenoDevourComponent) null;
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-failed-stomach-full"), victim, new EntityUid?(xeno), PopupType.SmallCaution);
      return false;
    }
    BuckleComponent comp;
    if (this.TryComp<BuckleComponent>(victim, out comp))
    {
      EntityUid? buckledTo = comp.BuckledTo;
      if (buckledTo.HasValue)
      {
        EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
        if (popup)
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-failed-target-buckled", ("strap", (object) valueOrDefault), ("target", (object) victim)), victim, new EntityUid?(xeno));
      }
    }
    return true;
  }

  private bool StartDevour(EntityUid xeno, EntityUid target)
  {
    XenoDevourComponent devour;
    if (!this.CanDevour(xeno, target, out devour, true))
      return false;
    DoAfterArgs args = new DoAfterArgs((IEntityManager) this.EntityManager, xeno, devour.DevourDelay, (DoAfterEvent) new XenoDevourDoAfterEvent(), new EntityUid?(xeno), new EntityUid?(target))
    {
      BreakOnMove = true,
      AttemptFrequency = AttemptFrequency.EveryTick,
      ForceVisible = true
    };
    IdentityEntity identityEntity = Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(xeno));
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-devour-start-self", (nameof (target), (object) identityEntity)), target, new EntityUid?(xeno));
    this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-start-target", ("user", (object) xeno)), xeno, target, PopupType.MediumCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept(xeno).RemovePlayerByAttachedEntity(target).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-start-observer", ("user", (object) xeno), (nameof (target), (object) identityEntity)), target, valueOrDefault, PopupType.SmallCaution);
      }
    }
    this._doAfter.TryStartDoAfter(args);
    return true;
  }

  private bool StartDevourPulled(EntityUid xeno)
  {
    EntityUid? pulling = (EntityUid?) this.CompOrNull<PullerComponent>(xeno)?.Pulling;
    if (!pulling.HasValue)
      return false;
    EntityUid valueOrDefault = pulling.GetValueOrDefault();
    return this.StartDevour(xeno, valueOrDefault);
  }

  private bool Regurgitate(
    Entity<DevouredComponent> devoured,
    Entity<XenoDevourComponent?> xeno,
    bool doFeedback = true)
  {
    BaseContainer container;
    if (!this.Resolve<XenoDevourComponent>((EntityUid) xeno, ref xeno.Comp) || !this._container.TryGetContainer((EntityUid) xeno, xeno.Comp.DevourContainerId, out container) || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) devoured.Owner, container))
      return true;
    RegurgitateEvent args = new RegurgitateEvent(this.GetNetEntity(xeno.Owner), this.GetNetEntity(devoured.Owner));
    this.RaiseLocalEvent<RegurgitateEvent>((EntityUid) xeno, args);
    if (doFeedback)
      this.DoFeedback((Entity<XenoDevourComponent>) ((EntityUid) xeno, xeno.Comp));
    return false;
  }

  private void RegurgitateAll(Entity<XenoDevourComponent> xeno)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) xeno, xeno.Comp.DevourContainerId, out container))
      return;
    bool flag = false;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      DevouredComponent comp;
      if (this.TryComp<DevouredComponent>(containedEntity, out comp) && this.Regurgitate((Entity<DevouredComponent>) (containedEntity, comp), (Entity<XenoDevourComponent>) ((EntityUid) xeno, (XenoDevourComponent) xeno), false))
        flag = true;
    }
    if (!flag)
      return;
    this.DoFeedback(xeno);
  }

  private void DoFeedback(Entity<XenoDevourComponent> xeno)
  {
    if (!this._net.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-hurl-out"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    this._audio.PlayPvs(xeno.Comp.RegurgitateSound, (EntityUid) xeno);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DevouredComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DevouredComponent, TransformComponent>();
    EntityUid uid;
    DevouredComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      BaseContainer container;
      XenoDevourComponent comp;
      if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid, comp2), out container) || !this.TryComp<XenoDevourComponent>(container.Owner, out comp) || container.ID != comp.DevourContainerId)
      {
        this.RemCompDeferred<DevouredComponent>(uid);
      }
      else
      {
        EntityUid owner = container.Owner;
        if (this._mobState.IsDead(uid))
        {
          this.Regurgitate((Entity<DevouredComponent>) (uid, comp1), (Entity<XenoDevourComponent>) (owner, comp));
        }
        else
        {
          if (!comp1.Warned && curTime >= comp1.WarnAt)
          {
            comp1.Warned = true;
            this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-regurgitate", ("target", (object) uid)), owner, owner, PopupType.MediumCaution);
          }
          if (curTime >= comp1.RegurgitateAt && this.Regurgitate((Entity<DevouredComponent>) (uid, comp1), (Entity<XenoDevourComponent>) (owner, comp)))
            this._popup.PopupEntity(this.Loc.GetString("cm-xeno-devour-hurl-out"), owner, owner, PopupType.MediumCaution);
        }
      }
    }
  }
}
