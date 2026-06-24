// Decompiled with JetBrains decompiler
// Type: Content.Shared.ActionBlocker.ActionBlockerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Content.Shared.Body.Events;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.ActionBlocker;

public sealed class ActionBlockerSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  private EntityQuery<ComplexInteractionComponent> _complexInteractionQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._complexInteractionQuery = this.GetEntityQuery<ComplexInteractionComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InputMoverComponent, ComponentStartup>(new ComponentEventHandler<InputMoverComponent, ComponentStartup>((object) this, __methodptr(OnMoverStartup)), (Type[]) null, (Type[]) null);
  }

  private void OnMoverStartup(EntityUid uid, InputMoverComponent component, ComponentStartup args)
  {
    this.UpdateCanMove(uid, component);
  }

  public bool CanMove(EntityUid uid, InputMoverComponent? component = null)
  {
    return this.Resolve<InputMoverComponent>(uid, ref component, false) && component.CanMove;
  }

  public bool UpdateCanMove(EntityUid uid, InputMoverComponent? component = null)
  {
    if (!this.Resolve<InputMoverComponent>(uid, ref component, false))
      return false;
    UpdateCanMoveEvent updateCanMoveEvent = new UpdateCanMoveEvent(uid);
    this.RaiseLocalEvent<UpdateCanMoveEvent>(uid, updateCanMoveEvent, false);
    if (component.CanMove == updateCanMoveEvent.Cancelled)
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    component.CanMove = !updateCanMoveEvent.Cancelled;
    return !updateCanMoveEvent.Cancelled;
  }

  public bool CanComplexInteract(EntityUid user) => this._complexInteractionQuery.HasComp(user);

  public bool CanInteract(EntityUid user, EntityUid? target)
  {
    if (!this.CanConsciouslyPerformAction(user))
      return false;
    InteractionAttemptEvent interactionAttemptEvent = new InteractionAttemptEvent(user, target);
    this.RaiseLocalEvent<InteractionAttemptEvent>(user, ref interactionAttemptEvent, false);
    if (interactionAttemptEvent.Cancelled)
      return false;
    if (target.HasValue)
    {
      EntityUid? nullable = target;
      EntityUid entityUid = user;
      if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      {
        GettingInteractedWithAttemptEvent withAttemptEvent = new GettingInteractedWithAttemptEvent(user, target);
        this.RaiseLocalEvent<GettingInteractedWithAttemptEvent>(target.Value, ref withAttemptEvent, false);
        return !withAttemptEvent.Cancelled;
      }
    }
    return true;
  }

  public bool CanUseHeldEntity(EntityUid user, EntityUid used)
  {
    UseAttemptEvent useAttemptEvent = new UseAttemptEvent(user, used);
    this.RaiseLocalEvent<UseAttemptEvent>(user, useAttemptEvent, false);
    if (useAttemptEvent.Cancelled)
      return false;
    GettingUsedAttemptEvent usedAttemptEvent = new GettingUsedAttemptEvent(user);
    this.RaiseLocalEvent<GettingUsedAttemptEvent>(used, usedAttemptEvent, false);
    return !usedAttemptEvent.Cancelled;
  }

  public bool CanConsciouslyPerformAction(EntityUid user)
  {
    ConsciousAttemptEvent consciousAttemptEvent = new ConsciousAttemptEvent(user);
    this.RaiseLocalEvent<ConsciousAttemptEvent>(user, ref consciousAttemptEvent, false);
    return !consciousAttemptEvent.Cancelled;
  }

  public bool CanThrow(EntityUid user, EntityUid itemUid)
  {
    ThrowAttemptEvent throwAttemptEvent = new ThrowAttemptEvent(user, itemUid);
    this.RaiseLocalEvent<ThrowAttemptEvent>(user, throwAttemptEvent, false);
    if (throwAttemptEvent.Cancelled)
      return false;
    ThrowItemAttemptEvent itemAttemptEvent = new ThrowItemAttemptEvent(user);
    this.RaiseLocalEvent<ThrowItemAttemptEvent>(itemUid, ref itemAttemptEvent, false);
    return !itemAttemptEvent.Cancelled;
  }

  public bool CanSpeak(EntityUid uid)
  {
    SpeakAttemptEvent speakAttemptEvent = new SpeakAttemptEvent(uid);
    this.RaiseLocalEvent<SpeakAttemptEvent>(uid, speakAttemptEvent, true);
    return !speakAttemptEvent.Cancelled;
  }

  public bool CanDrop(EntityUid uid, EntityUid? held = null)
  {
    DropAttemptEvent dropAttemptEvent1 = new DropAttemptEvent();
    this.RaiseLocalEvent<DropAttemptEvent>(uid, dropAttemptEvent1, false);
    if (held.HasValue)
    {
      RMCItemDropAttemptEvent dropAttemptEvent2 = new RMCItemDropAttemptEvent();
      this.RaiseLocalEvent<RMCItemDropAttemptEvent>(held.Value, ref dropAttemptEvent2, false);
      if (dropAttemptEvent2.Cancelled)
        return false;
    }
    return !dropAttemptEvent1.Cancelled;
  }

  public bool CanPickup(EntityUid user, EntityUid item)
  {
    PickupAttemptEvent pickupAttemptEvent = new PickupAttemptEvent(user, item);
    this.RaiseLocalEvent<PickupAttemptEvent>(user, pickupAttemptEvent, false);
    if (pickupAttemptEvent.Cancelled)
      return false;
    GettingPickedUpAttemptEvent pickedUpAttemptEvent = new GettingPickedUpAttemptEvent(user, item);
    this.RaiseLocalEvent<GettingPickedUpAttemptEvent>(item, pickedUpAttemptEvent, false);
    return !pickedUpAttemptEvent.Cancelled;
  }

  public bool CanEmote(EntityUid uid)
  {
    EmoteAttemptEvent emoteAttemptEvent = new EmoteAttemptEvent(uid);
    this.RaiseLocalEvent<EmoteAttemptEvent>(uid, emoteAttemptEvent, true);
    return !emoteAttemptEvent.Cancelled;
  }

  public bool CanAttack(
    EntityUid uid,
    EntityUid? target = null,
    Entity<MeleeWeaponComponent>? weapon = null,
    bool disarm = false)
  {
    if (target.HasValue && this._container.IsEntityInContainer(target.Value, (MetaDataComponent) null))
      return false;
    BaseContainer baseContainer;
    this._container.TryGetOuterContainer(uid, this.Transform(uid), ref baseContainer);
    EntityUid? nullable1;
    if (target.HasValue)
    {
      EntityUid? nullable2 = target;
      nullable1 = baseContainer?.Owner;
      if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (EntityUid.op_Inequality(nullable2.GetValueOrDefault(), nullable1.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0 && this._container.IsEntityInContainer(uid, (MetaDataComponent) null))
      {
        CanAttackFromContainerEvent fromContainerEvent = new CanAttackFromContainerEvent(uid, target);
        this.RaiseLocalEvent<CanAttackFromContainerEvent>(uid, fromContainerEvent, false);
        if (!fromContainerEvent.CanAttack)
          return false;
      }
    }
    AttackAttemptEvent attackAttemptEvent = new AttackAttemptEvent(uid, target, weapon, disarm);
    this.RaiseLocalEvent<AttackAttemptEvent>(uid, attackAttemptEvent, false);
    if (attackAttemptEvent.Cancelled)
      return false;
    if (!target.HasValue)
      return true;
    GettingAttackedAttemptEvent attackedAttemptEvent;
    ref GettingAttackedAttemptEvent local = ref attackedAttemptEvent;
    EntityUid Attacker = uid;
    Entity<MeleeWeaponComponent>? nullable3 = weapon;
    EntityUid? Weapon;
    if (!nullable3.HasValue)
    {
      nullable1 = new EntityUid?();
      Weapon = nullable1;
    }
    else
      Weapon = new EntityUid?(Entity<MeleeWeaponComponent>.op_Implicit(nullable3.GetValueOrDefault()));
    int num = disarm ? 1 : 0;
    local = new GettingAttackedAttemptEvent(Attacker, Weapon, num != 0);
    this.RaiseLocalEvent<GettingAttackedAttemptEvent>(target.Value, ref attackedAttemptEvent, false);
    return !attackedAttemptEvent.Cancelled;
  }

  public bool CanChangeDirection(EntityUid uid)
  {
    ChangeDirectionAttemptEvent directionAttemptEvent = new ChangeDirectionAttemptEvent(uid);
    this.RaiseLocalEvent<ChangeDirectionAttemptEvent>(uid, directionAttemptEvent, false);
    return !directionAttemptEvent.Cancelled;
  }

  public bool CanShiver(EntityUid uid)
  {
    ShiverAttemptEvent shiverAttemptEvent = new ShiverAttemptEvent(uid);
    this.RaiseLocalEvent<ShiverAttemptEvent>(uid, ref shiverAttemptEvent, false);
    return !shiverAttemptEvent.Cancelled;
  }

  public bool CanSweat(EntityUid uid)
  {
    SweatAttemptEvent sweatAttemptEvent = new SweatAttemptEvent(uid);
    this.RaiseLocalEvent<SweatAttemptEvent>(uid, ref sweatAttemptEvent, false);
    return !sweatAttemptEvent.Cancelled;
  }
}
