// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableToggleableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.DoAfter;
using Content.Shared.Fluids;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableToggleableSystem : EntitySystem
{
  [Dependency]
  private ActionContainerSystem _actionContainerSystem;
  [Dependency]
  private EntityLookupSystem _entityLookupSystem;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelistSystem;
  [Dependency]
  private MetaDataSystem _metaDataSystem;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private AttachableHolderSystem _attachableHolderSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private UseDelaySystem _useDelaySystem;
  private const string attachableToggleUseDelayID = "RMCAttachableToggle";
  private const int bracingInvalidCollisionGroup = 74;
  private const int bracingRequiredCollisionGroup = 4;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableToggleableComponent, ActivateInWorldEvent>(new EntityEventRefHandler<AttachableToggleableComponent, ActivateInWorldEvent>(this.OnActivateInWorld), after: new Type[1]
    {
      typeof (CMGunSystem)
    });
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableAlteredEvent>(this.OnAttachableAltered), after: new Type[1]
    {
      typeof (AttachableModifiersSystem)
    });
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleableInterruptEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleableInterruptEvent>(this.OnAttachableToggleableInterrupt));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleActionEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleActionEvent>(this.OnAttachableToggleAction));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleDoAfterEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleDoAfterEvent>(this.OnAttachableToggleDoAfter));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleStartedEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleStartedEvent>(this.OnAttachableToggleStarted));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttemptShootEvent>(new EntityEventRefHandler<AttachableToggleableComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GunShotEvent>>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GunShotEvent>>(this.OnGunShot));
    this.SubscribeLocalEvent<AttachableToggleableComponent, GunShotEvent>(new EntityEventRefHandler<AttachableToggleableComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<AttachableToggleableComponent, ToggleActionEvent>(new EntityEventRefHandler<AttachableToggleableComponent, ToggleActionEvent>(this.OnToggleAction), new Type[1]
    {
      typeof (SharedHandheldLightSystem)
    });
    this.SubscribeLocalEvent<AttachableToggleableComponent, GrantAttachableActionsEvent>(new EntityEventRefHandler<AttachableToggleableComponent, GrantAttachableActionsEvent>(this.OnGrantAttachableActions));
    this.SubscribeLocalEvent<AttachableToggleableComponent, RemoveAttachableActionsEvent>(new EntityEventRefHandler<AttachableToggleableComponent, RemoveAttachableActionsEvent>(this.OnRemoveAttachableActions));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<HandDeselectedEvent>>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<HandDeselectedEvent>>(this.OnHandDeselected));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GotEquippedHandEvent>>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GotEquippedHandEvent>>(this.OnGotEquippedHand));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GotUnequippedHandEvent>>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GotUnequippedHandEvent>>(this.OnGotUnequippedHand));
    this.SubscribeLocalEvent<AttachableToggleableComponent, SprayAttemptEvent>(new EntityEventRefHandler<AttachableToggleableComponent, SprayAttemptEvent>(this.OnSprayAttempt));
    this.SubscribeLocalEvent<AttachableToggleableComponent, DroppedEvent>(new EntityEventRefHandler<AttachableToggleableComponent, DroppedEvent>(this.OnDropped));
    this.SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<DroppedEvent>>(new EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<DroppedEvent>>(this.OnDropped));
    this.SubscribeLocalEvent<AttachableMovementLockedComponent, MoveInputEvent>(new EntityEventRefHandler<AttachableMovementLockedComponent, MoveInputEvent>(this.OnAttachableMovementLockedMoveInput));
    this.SubscribeLocalEvent<AttachableToggleableSimpleActivateComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableToggleableSimpleActivateComponent, AttachableAlteredEvent>(this.OnAttachableAltered), after: new Type[1]
    {
      typeof (AttachableModifiersSystem)
    });
    this.SubscribeLocalEvent<AttachableToggleablePreventShootComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableToggleablePreventShootComponent, AttachableAlteredEvent>(this.OnAttachableAltered), after: new Type[1]
    {
      typeof (AttachableModifiersSystem)
    });
    this.SubscribeLocalEvent<AttachableGunPreventShootComponent, AttemptShootEvent>(new EntityEventRefHandler<AttachableGunPreventShootComponent, AttemptShootEvent>(this.OnAttemptShoot));
  }

  private void OnAttachableAltered(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        attachable.Comp.Attached = true;
        break;
      case AttachableAlteredType.Detached:
        AttachableHolderComponent comp1;
        if (attachable.Comp.SupercedeHolder && this.TryComp<AttachableHolderComponent>(args.Holder, out comp1))
        {
          EntityUid? supercedingAttachable = comp1.SupercedingAttachable;
          EntityUid owner = attachable.Owner;
          if ((supercedingAttachable.HasValue ? (supercedingAttachable.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
            this._attachableHolderSystem.SetSupercedingAttachable((Entity<AttachableHolderComponent>) (args.Holder, comp1), new EntityUid?());
        }
        if (attachable.Comp.Active)
        {
          AttachableAlteredEvent args1 = args with
          {
            Alteration = AttachableAlteredType.DetachedDeactivated
          };
          this.RaiseLocalEvent<AttachableAlteredEvent>(attachable.Owner, ref args1);
        }
        attachable.Comp.Attached = false;
        attachable.Comp.Active = false;
        this.Dirty<AttachableToggleableComponent>(attachable);
        break;
      case AttachableAlteredType.Unwielded:
        if (attachable.Comp.WieldedOnly && attachable.Comp.Active)
        {
          this.Toggle(attachable, args.User, attachable.Comp.DoInterrupt);
          break;
        }
        break;
    }
    ActionComponent comp2;
    if (!attachable.Comp.Action.HasValue || !this.TryComp<ActionComponent>(attachable.Comp.Action, out comp2))
      return;
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? action1 = attachable.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = attachable.Comp.Active ? 1 : 0;
    actionsSystem.SetToggled(action2, num != 0);
    this._actionsSystem.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) (attachable.Comp.Action.Value, comp2)), attachable.Comp.Attached);
  }

  private void OnAttachableAltered(
    Entity<AttachableToggleableSimpleActivateComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    if (!args.User.HasValue)
      return;
    switch (args.Alteration)
    {
      case AttachableAlteredType.Activated:
        this.RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, true));
        break;
      case AttachableAlteredType.Deactivated:
        this.RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, true));
        break;
      case AttachableAlteredType.DetachedDeactivated:
        this.RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, true));
        break;
    }
  }

  private void OnAttachableAltered(
    Entity<AttachableToggleablePreventShootComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    AttachableToggleableComponent comp1;
    if (!this.TryComp<AttachableToggleableComponent>(attachable.Owner, out comp1))
      return;
    AttachableGunPreventShootComponent comp2;
    this.EnsureComp<AttachableGunPreventShootComponent>(args.Holder, out comp2);
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        comp2.Message = attachable.Comp.Message;
        comp2.PreventShoot = attachable.Comp.ShootWhenActive && !comp1.Active || !attachable.Comp.ShootWhenActive && comp1.Active;
        break;
      case AttachableAlteredType.Detached:
        comp2.Message = "";
        comp2.PreventShoot = false;
        break;
      case AttachableAlteredType.Activated:
        comp2.PreventShoot = !attachable.Comp.ShootWhenActive;
        break;
      case AttachableAlteredType.Deactivated:
        comp2.PreventShoot = attachable.Comp.ShootWhenActive;
        break;
      case AttachableAlteredType.DetachedDeactivated:
        comp2.PreventShoot = false;
        break;
    }
    this.Dirty(args.Holder, (IComponent) comp2);
  }

  private void OnGotEquippedHand(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableRelayedEvent<GotEquippedHandEvent> args)
  {
    if (!attachable.Comp.Attached)
      return;
    args.Args.Handled = true;
    GrantAttachableActionsEvent args1 = new GrantAttachableActionsEvent(args.Args.User);
    this.RaiseLocalEvent<GrantAttachableActionsEvent>((EntityUid) attachable, ref args1);
  }

  private void OnActivateInWorld(
    Entity<AttachableToggleableComponent> attachable,
    ref ActivateInWorldEvent args)
  {
    if (!attachable.Comp.AttachedOnly || attachable.Comp.Attached)
      return;
    args.Handled = true;
  }

  private void OnAttemptShoot(
    Entity<AttachableToggleableComponent> attachable,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled)
      return;
    if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached)
    {
      args.Cancelled = true;
    }
    else
    {
      EntityUid? holderUid;
      WieldableComponent comp;
      if (!attachable.Comp.WieldedUseOnly || this._attachableHolderSystem.TryGetHolder(attachable.Owner, out holderUid) && this.TryComp<WieldableComponent>(holderUid, out comp) && comp.Wielded)
        return;
      args.Cancelled = true;
      if (!holderUid.HasValue)
        return;
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-attachable-shoot-fail-not-wielded", ("holder", (object) holderUid), (nameof (attachable), (object) attachable)), args.User, new EntityUid?(args.User));
    }
  }

  private void OnGunShot(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableRelayedEvent<GunShotEvent> args)
  {
    this.CheckUserBreakOnRotate((Entity<AttachableDirectionLockedComponent>) args.Args.User);
    this.CheckUserBreakOnFullRotate((Entity<AttachableSideLockedComponent>) args.Args.User, args.Args.FromCoordinates, args.Args.ToCoordinates);
  }

  private void OnGunShot(Entity<AttachableToggleableComponent> attachable, ref GunShotEvent args)
  {
    this.CheckUserBreakOnRotate((Entity<AttachableDirectionLockedComponent>) args.User);
    this.CheckUserBreakOnFullRotate((Entity<AttachableSideLockedComponent>) args.User, args.FromCoordinates, args.ToCoordinates);
  }

  private void OnAttemptShoot(
    Entity<AttachableGunPreventShootComponent> gun,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || !gun.Comp.PreventShoot)
      return;
    args.Cancelled = true;
    this._popupSystem.PopupClient(gun.Comp.Message, args.User, new EntityUid?(args.User));
  }

  private void OnHandDeselected(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableRelayedEvent<HandDeselectedEvent> args)
  {
    if (!attachable.Comp.Attached)
      return;
    args.Args.Handled = true;
    if (!attachable.Comp.NeedHand || !attachable.Comp.Active)
      return;
    this.Toggle(attachable, new EntityUid?(args.Args.User), attachable.Comp.DoInterrupt);
  }

  private void OnAttachableToggleableInterrupt(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableToggleableInterruptEvent args)
  {
    if (!attachable.Comp.Active)
      return;
    this.Toggle(attachable, new EntityUid?(args.User), attachable.Comp.DoInterrupt);
  }

  private void OnGotUnequippedHand(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableRelayedEvent<GotUnequippedHandEvent> args)
  {
    if (!attachable.Comp.Attached)
      return;
    args.Args.Handled = true;
    if ((attachable.Comp.NeedHand || attachable.Comp.BreakOnDrop) && attachable.Comp.Active)
      this.Toggle(attachable, new EntityUid?(args.Args.User), attachable.Comp.DoInterrupt);
    RemoveAttachableActionsEvent args1 = new RemoveAttachableActionsEvent(args.Args.User);
    this.RaiseLocalEvent<RemoveAttachableActionsEvent>((EntityUid) attachable, ref args1);
  }

  private void OnSprayAttempt(
    Entity<AttachableToggleableComponent> attachable,
    ref SprayAttemptEvent args)
  {
    if (args.Cancelled || !attachable.Comp.AttachedOnly || attachable.Comp.Attached)
      return;
    args.Cancelled = true;
  }

  private void OnDropped(Entity<AttachableToggleableComponent> attachable, ref DroppedEvent args)
  {
    if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached || !attachable.Comp.BreakOnDrop || !attachable.Comp.Active)
      return;
    this.Toggle(attachable, new EntityUid?(args.User));
  }

  private void OnDropped(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableRelayedEvent<DroppedEvent> args)
  {
    if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached || !attachable.Comp.BreakOnDrop || !attachable.Comp.Active)
      return;
    this.Toggle(attachable, new EntityUid?(args.Args.User));
  }

  private void OnAttachableMovementLockedMoveInput(
    Entity<AttachableMovementLockedComponent> user,
    ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    for (int index = user.Comp.AttachableList.Count - 1; index >= 0; --index)
    {
      EntityUid attachable = user.Comp.AttachableList[index];
      AttachableToggleableComponent comp;
      if (this.TryComp<AttachableToggleableComponent>(attachable, out comp) && comp.Active && comp.BreakOnMove)
        this.Toggle((Entity<AttachableToggleableComponent>) (attachable, comp), new EntityUid?(user.Owner), comp.DoInterrupt);
    }
  }

  private void CheckUserBreakOnRotate(Entity<AttachableDirectionLockedComponent?> user)
  {
    if (user.Comp == null)
    {
      AttachableDirectionLockedComponent comp;
      if (!this.TryComp<AttachableDirectionLockedComponent>(user.Owner, out comp))
        return;
      user.Comp = comp;
    }
    Angle localRotation = this.Transform(user.Owner).LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    Direction? lockedDirection = user.Comp.LockedDirection;
    Direction valueOrDefault = lockedDirection.GetValueOrDefault();
    if (cardinalDir == valueOrDefault & lockedDirection.HasValue)
      return;
    for (int index = user.Comp.AttachableList.Count - 1; index >= 0; --index)
    {
      EntityUid attachable = user.Comp.AttachableList[index];
      AttachableToggleableComponent comp;
      if (this.TryComp<AttachableToggleableComponent>(attachable, out comp) && comp.Active && comp.BreakOnRotate)
        this.Toggle((Entity<AttachableToggleableComponent>) (attachable, comp), new EntityUid?(user.Owner), comp.DoInterrupt);
    }
  }

  private void CheckUserBreakOnFullRotate(
    Entity<AttachableSideLockedComponent?> user,
    EntityCoordinates playerPos,
    EntityCoordinates targetPos)
  {
    if (user.Comp == null)
    {
      AttachableSideLockedComponent comp;
      if (!this.TryComp<AttachableSideLockedComponent>(user.Owner, out comp))
        return;
      user.Comp = comp;
    }
    if (!user.Comp.LockedDirection.HasValue)
      return;
    Angle angle = DirectionExtensions.ToAngle(user.Comp.LockedDirection.Value);
    MapCoordinates mapCoordinates = this._transformSystem.ToMapCoordinates(playerPos);
    Angle worldAngle = DirectionExtensions.ToWorldAngle(this._transformSystem.ToMapCoordinates(targetPos).Position - mapCoordinates.Position);
    double num = (((Angle) ref worldAngle).Degrees - ((Angle) ref angle).Degrees + 180.0 + 360.0) % 360.0 - 180.0;
    if (num > -90.0 && num < 90.0)
      return;
    for (int index = user.Comp.AttachableList.Count - 1; index >= 0; --index)
    {
      EntityUid attachable = user.Comp.AttachableList[index];
      AttachableToggleableComponent comp;
      if (this.TryComp<AttachableToggleableComponent>(attachable, out comp) && comp.Active && comp.BreakOnFullRotate)
        this.Toggle((Entity<AttachableToggleableComponent>) (attachable, comp), new EntityUid?(user.Owner), comp.DoInterrupt);
    }
  }

  private void OnAttachableToggleStarted(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableToggleStartedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.User) || !this.CanStartToggleDoAfter(attachable, ref args))
      return;
    string popupText = this.Loc.GetString((string) (attachable.Comp.Active ? attachable.Comp.DeactivatePopupText : attachable.Comp.ActivatePopupText), (nameof (attachable), (object) attachable.Owner));
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, this.GetToggleDoAfter(attachable, args.Holder, args.User, ref popupText), (DoAfterEvent) new AttachableToggleDoAfterEvent(args.SlotId, popupText), new EntityUid?((EntityUid) attachable), new EntityUid?(attachable.Owner), new EntityUid?(args.Holder))
    {
      NeedHand = attachable.Comp.DoAfterNeedHand,
      BreakOnMove = attachable.Comp.DoAfterBreakOnMove
    });
    this.Dirty<AttachableToggleableComponent>(attachable);
  }

  private bool CanStartToggleDoAfter(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableToggleStartedEvent args,
    bool silent = false)
  {
    UseDelayComponent comp1;
    if (this.TryComp<UseDelayComponent>(attachable.Owner, out comp1) && this._useDelaySystem.IsDelayed((Entity<UseDelayComponent>) (attachable.Owner, comp1), "RMCAttachableToggle"))
      return false;
    EntityUid? userUid;
    this._attachableHolderSystem.TryGetUser(attachable.Owner, out userUid);
    if (attachable.Comp.HeldOnlyActivate && !attachable.Comp.Active && (!userUid.HasValue || !this._handsSystem.IsHolding((Entity<HandsComponent>) userUid.Value, new EntityUid?(args.Holder), out string _)))
    {
      if (!silent)
        this._popupSystem.PopupClient(this.Loc.GetString("rmc-attachable-activation-fail-not-held", ("holder", (object) args.Holder), (nameof (attachable), (object) attachable)), args.User, new EntityUid?(args.User));
      return false;
    }
    if (attachable.Comp.UserOnly)
    {
      EntityUid? nullable = userUid;
      EntityUid user = args.User;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != user ? 1 : 0) : 1) != 0)
      {
        if (!silent)
          this._popupSystem.PopupClient(this.Loc.GetString("rmc-attachable-activation-fail-not-owned", ("holder", (object) args.Holder), (nameof (attachable), (object) attachable)), args.User, new EntityUid?(args.User));
        return false;
      }
    }
    WieldableComponent comp2;
    if (attachable.Comp.Active || !attachable.Comp.WieldedOnly || this.TryComp<WieldableComponent>(args.Holder, out comp2) && comp2.Wielded)
      return true;
    if (!silent)
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-attachable-activation-fail-not-wielded", ("holder", (object) args.Holder), (nameof (attachable), (object) attachable)), args.User, new EntityUid?(args.User));
    return false;
  }

  private TimeSpan GetToggleDoAfter(
    Entity<AttachableToggleableComponent> attachable,
    EntityUid holderUid,
    EntityUid userUid,
    ref string popupText)
  {
    TransformComponent comp1;
    if (!this.TryComp(holderUid, out comp1) || !comp1.ParentUid.Valid)
      return TimeSpan.FromSeconds(0.0);
    float num = comp1.ParentUid == userUid ? 0.0f : 0.5f;
    TransformComponent comp2;
    if (attachable.Comp.InstantToggle == AttachableInstantToggleConditions.Brace && !attachable.Comp.Active && !(comp1.ParentUid != userUid) && this.TryComp(userUid, out comp2))
    {
      EntityCoordinates coords = comp2.Coordinates;
      Func<EntityCoordinates, EntityCoordinates, bool> comparer = (Func<EntityCoordinates, EntityCoordinates, bool>) ((userCoords, entCoords) => false);
      Vector2 vector2 = new Vector2(0.0f, 0.0f);
      Func<HashSet<EntityUid>, EntityUid?> func = (Func<HashSet<EntityUid>, EntityUid?>) (ents =>
      {
        foreach (EntityUid ent in ents)
        {
          FixturesComponent comp3;
          if (this.TryComp<FixturesComponent>(ent, out comp3) && this.Transform(ent).Anchored)
          {
            foreach (Fixture fixture in comp3.Fixtures.Values)
            {
              if ((fixture.CollisionLayer & 74) == 0 && (fixture.CollisionLayer & 4) != 0 && comparer(coords, this.Transform(ent).Coordinates))
                return new EntityUid?(ent);
            }
          }
        }
        return new EntityUid?();
      });
      Angle localRotation = comp2.LocalRotation;
      switch ((int) ((Angle) ref localRotation).GetCardinalDir())
      {
        case 0:
          comparer = (Func<EntityCoordinates, EntityCoordinates, bool>) ((userCoords, entCoords) => (double) entCoords.Y < (double) userCoords.Y);
          vector2 = new Vector2(0.0f, -0.7f);
          break;
        case 2:
          comparer = (Func<EntityCoordinates, EntityCoordinates, bool>) ((userCoords, entCoords) => (double) entCoords.X > (double) userCoords.X);
          vector2 = new Vector2(0.7f, 0.0f);
          break;
        case 4:
          comparer = (Func<EntityCoordinates, EntityCoordinates, bool>) ((userCoords, entCoords) => (double) entCoords.Y > (double) userCoords.Y);
          vector2 = new Vector2(0.0f, 0.7f);
          break;
        case 6:
          comparer = (Func<EntityCoordinates, EntityCoordinates, bool>) ((userCoords, entCoords) => (double) entCoords.X < (double) userCoords.X);
          vector2 = new Vector2(-0.7f, 0.0f);
          break;
      }
      EntityUid? nullable1 = func(this._entityLookupSystem.GetEntitiesInRange(coords, 0.5f, LookupFlags.Dynamic | LookupFlags.Static));
      if (nullable1.HasValue)
      {
        popupText = this.Loc.GetString("attachable-popup-activate-deploy-on-generic", (nameof (attachable), (object) attachable.Owner), ("surface", (object) nullable1));
        return TimeSpan.FromSeconds(0.0);
      }
      coords = new EntityCoordinates(coords.EntityId, coords.Position + vector2);
      EntityUid? nullable2 = func(this._entityLookupSystem.GetEntitiesInRange(coords, 0.5f, LookupFlags.Dynamic | LookupFlags.Static));
      if (nullable2.HasValue)
      {
        popupText = this.Loc.GetString("attachable-popup-activate-deploy-on-generic", (nameof (attachable), (object) attachable.Owner), ("surface", (object) nullable2));
        return TimeSpan.FromSeconds(0.0);
      }
      popupText = this.Loc.GetString("attachable-popup-activate-deploy-on-ground", (nameof (attachable), (object) attachable.Owner));
    }
    return TimeSpan.FromSeconds((double) Math.Max((!attachable.Comp.DeactivateDoAfter.HasValue || !attachable.Comp.Active ? attachable.Comp.DoAfter : attachable.Comp.DeactivateDoAfter.Value) + num, 0.0f));
  }

  private void OnAttachableToggleDoAfter(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableToggleDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    nullable = args.Used;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
    AttachableHolderComponent comp1;
    InputMoverComponent comp2;
    if (!this.HasComp<AttachableToggleableComponent>(valueOrDefault1) || !this.TryComp<AttachableHolderComponent>(args.Used, out comp1) || !attachable.Comp.Active && this.TryComp<InputMoverComponent>(args.User, out comp2) && (comp2.HeldMoveButtons & MoveButtons.AnyDirection) != MoveButtons.None && !attachable.Comp.ActivateOnMove)
      return;
    this.FinishToggle(attachable, (Entity<AttachableHolderComponent>) (valueOrDefault2, comp1), args.SlotId, new EntityUid?(args.User), args.PopupText);
    args.Handled = true;
    this.Dirty<AttachableToggleableComponent>(attachable);
  }

  private void RemoveUnusedLocks(
    Entity<AttachableToggleableComponent> attachable,
    EntityUid? userUid)
  {
    if (!userUid.HasValue)
      return;
    AttachableMovementLockedComponent comp1;
    if (attachable.Comp.BreakOnMove && this.TryComp<AttachableMovementLockedComponent>(userUid.Value, out comp1))
    {
      comp1.AttachableList.Remove(attachable.Owner);
      if (comp1.AttachableList.Count == 0)
        this.RemCompDeferred<AttachableMovementLockedComponent>(userUid.Value);
    }
    AttachableDirectionLockedComponent comp2;
    if (attachable.Comp.BreakOnRotate && this.TryComp<AttachableDirectionLockedComponent>(userUid.Value, out comp2))
    {
      comp2.AttachableList.Remove(attachable.Owner);
      if (comp2.AttachableList.Count == 0)
        this.RemCompDeferred<AttachableDirectionLockedComponent>(userUid.Value);
    }
    AttachableSideLockedComponent comp3;
    if (!attachable.Comp.BreakOnFullRotate || !this.TryComp<AttachableSideLockedComponent>(userUid.Value, out comp3))
      return;
    comp3.AttachableList.Remove(attachable.Owner);
    if (comp3.AttachableList.Count != 0)
      return;
    this.RemCompDeferred<AttachableSideLockedComponent>(userUid.Value);
  }

  private void FinishToggle(
    Entity<AttachableToggleableComponent> attachable,
    Entity<AttachableHolderComponent> holder,
    string slotId,
    EntityUid? userUid,
    string popupText,
    bool interrupted = false)
  {
    attachable.Comp.Active = !attachable.Comp.Active;
    AttachableAlteredType Alteration = attachable.Comp.Active ? AttachableAlteredType.Activated : (interrupted ? AttachableAlteredType.Interrupted : AttachableAlteredType.Deactivated);
    AttachableAlteredEvent args1 = new AttachableAlteredEvent(holder.Owner, Alteration, userUid);
    this.RaiseLocalEvent<AttachableAlteredEvent>(attachable.Owner, ref args1);
    AttachableHolderAttachablesAlteredEvent args2 = new AttachableHolderAttachablesAlteredEvent(attachable.Owner, slotId, Alteration);
    this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref args2);
    this._useDelaySystem.SetLength((Entity<UseDelayComponent>) attachable.Owner, attachable.Comp.UseDelay, "RMCAttachableToggle");
    this._useDelaySystem.TryResetDelay(attachable.Owner, id: "RMCAttachableToggle");
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? nullable1 = attachable.Comp.Action;
    Entity<ActionComponent>? action = nullable1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.StartUseDelay(action);
    if (attachable.Comp.ShowTogglePopup && userUid.HasValue)
      this._popupSystem.PopupClient(popupText, userUid.Value, new EntityUid?(userUid.Value));
    this._audioSystem.PlayPredicted(attachable.Comp.Active ? attachable.Comp.ActivateSound : attachable.Comp.DeactivateSound, (EntityUid) attachable, userUid);
    if (!attachable.Comp.Active)
    {
      if (attachable.Comp.SupercedeHolder)
      {
        nullable1 = holder.Comp.SupercedingAttachable;
        EntityUid owner = attachable.Owner;
        if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
        {
          AttachableHolderSystem attachableHolderSystem = this._attachableHolderSystem;
          Entity<AttachableHolderComponent> holder1 = holder;
          nullable1 = new EntityUid?();
          EntityUid? supercedingAttachable = nullable1;
          attachableHolderSystem.SetSupercedingAttachable(holder1, supercedingAttachable);
        }
      }
      this.RemoveUnusedLocks(attachable, userUid);
    }
    else
    {
      if (attachable.Comp.BreakOnMove && userUid.HasValue)
        this.EnsureComp<AttachableMovementLockedComponent>(userUid.Value).AttachableList.Add(attachable.Owner);
      Angle localRotation;
      if (attachable.Comp.BreakOnRotate && userUid.HasValue)
      {
        AttachableDirectionLockedComponent directionLockedComponent1 = this.EnsureComp<AttachableDirectionLockedComponent>(userUid.Value);
        directionLockedComponent1.AttachableList.Add(attachable.Owner);
        if (!directionLockedComponent1.LockedDirection.HasValue)
        {
          AttachableDirectionLockedComponent directionLockedComponent2 = directionLockedComponent1;
          localRotation = this.Transform(userUid.Value).LocalRotation;
          Direction? nullable2 = new Direction?(((Angle) ref localRotation).GetCardinalDir());
          directionLockedComponent2.LockedDirection = nullable2;
        }
      }
      if (attachable.Comp.BreakOnFullRotate && userUid.HasValue)
      {
        AttachableSideLockedComponent sideLockedComponent1 = this.EnsureComp<AttachableSideLockedComponent>(userUid.Value);
        sideLockedComponent1.AttachableList.Add(attachable.Owner);
        if (!sideLockedComponent1.LockedDirection.HasValue)
        {
          AttachableSideLockedComponent sideLockedComponent2 = sideLockedComponent1;
          localRotation = this.Transform(userUid.Value).LocalRotation;
          Direction? nullable3 = new Direction?(((Angle) ref localRotation).GetCardinalDir());
          sideLockedComponent2.LockedDirection = nullable3;
        }
      }
      if (!attachable.Comp.SupercedeHolder)
        return;
      AttachableToggleableComponent comp;
      if (holder.Comp.SupercedingAttachable.HasValue && this.TryComp<AttachableToggleableComponent>(holder.Comp.SupercedingAttachable, out comp))
      {
        comp.Active = false;
        ref AttachableAlteredEvent local = ref args1;
        EntityUid owner = holder.Owner;
        nullable1 = new EntityUid?();
        EntityUid? User = nullable1;
        local = new AttachableAlteredEvent(owner, AttachableAlteredType.Deactivated, User);
        this.RaiseLocalEvent<AttachableAlteredEvent>(holder.Comp.SupercedingAttachable.Value, ref args1);
        string slotId1;
        if (this._attachableHolderSystem.TryGetSlotId(holder.Owner, attachable.Owner, out slotId1))
        {
          args2 = new AttachableHolderAttachablesAlteredEvent(holder.Comp.SupercedingAttachable.Value, slotId1, AttachableAlteredType.Deactivated);
          this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref args2);
        }
      }
      this._attachableHolderSystem.SetSupercedingAttachable(holder, new EntityUid?(attachable.Owner));
    }
  }

  private void Toggle(
    Entity<AttachableToggleableComponent> attachable,
    EntityUid? user,
    bool interrupted = false)
  {
    EntityUid? holderUid;
    AttachableHolderComponent comp;
    string slotId;
    if (!this._attachableHolderSystem.TryGetHolder(attachable.Owner, out holderUid) || !this.TryComp<AttachableHolderComponent>(holderUid, out comp) || !this._attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out slotId))
      return;
    this.FinishToggle(attachable, (Entity<AttachableHolderComponent>) (holderUid.Value, comp), slotId, user, this.Loc.GetString((string) (attachable.Comp.Active ? attachable.Comp.DeactivatePopupText : attachable.Comp.ActivatePopupText), (nameof (attachable), (object) attachable.Owner)), interrupted);
    this.Dirty<AttachableToggleableComponent>(attachable);
  }

  private void OnGrantAttachableActions(
    Entity<AttachableToggleableComponent> ent,
    ref GrantAttachableActionsEvent args)
  {
    this.GrantAttachableActions(ent, args.User);
    this.RelayAttachableActions(ent, args.User);
  }

  private void GrantAttachableActions(
    Entity<AttachableToggleableComponent> ent,
    EntityUid user,
    bool doSecondTry = true)
  {
    ActionsContainerComponent comp;
    if (!this.TryComp<ActionsContainerComponent>(ent.Owner, out comp) || comp.Container == null)
    {
      this.EnsureComp<ActionsContainerComponent>(ent.Owner);
      if (!doSecondTry)
        return;
      this.GrantAttachableActions(ent, user, false);
    }
    else
    {
      bool hasValue = ent.Comp.Action.HasValue;
      this._actionContainerSystem.EnsureAction((EntityUid) ent, ref ent.Comp.Action, ent.Comp.ActionId, comp);
      EntityUid? action1 = ent.Comp.Action;
      if (!action1.HasValue)
        return;
      EntityUid valueOrDefault = action1.GetValueOrDefault();
      this._actionsSystem.GrantContainedAction((Entity<ActionsComponent>) user, (Entity<ActionsContainerComponent>) ent.Owner, valueOrDefault);
      if (hasValue)
        return;
      this._metaDataSystem.SetEntityName(valueOrDefault, ent.Comp.ActionName);
      this._metaDataSystem.SetEntityDescription(valueOrDefault, ent.Comp.ActionDesc);
      Entity<ActionComponent>? action2 = this._actionsSystem.GetAction(new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault));
      if (action2.HasValue)
      {
        Entity<ActionComponent> ent1 = action2.GetValueOrDefault().AsNullable();
        this._actionsSystem.SetIcon(ent1, ent.Comp.Icon);
        this._actionsSystem.SetIconOn(ent1, ent.Comp.IconActive);
        this._actionsSystem.SetEnabled(new Entity<ActionComponent>?(ent1), ent.Comp.Attached);
        this._actionsSystem.SetUseDelay(new Entity<ActionComponent>?(ent1), new TimeSpan?(ent.Comp.UseDelay));
      }
      this.Dirty<AttachableToggleableComponent>(ent);
    }
  }

  private void RelayAttachableActions(
    Entity<AttachableToggleableComponent> attachable,
    EntityUid user)
  {
    ActionsContainerComponent comp;
    if (attachable.Comp.ActionsToRelayWhitelist == null || !this.TryComp<ActionsContainerComponent>(attachable.Owner, out comp))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) comp.Container.ContainedEntities)
    {
      if (this._entityWhitelistSystem.IsWhitelistPass(attachable.Comp.ActionsToRelayWhitelist, containedEntity))
        this._actionsSystem.GrantContainedAction((Entity<ActionsComponent>) user, (Entity<ActionsContainerComponent>) (attachable.Owner, comp), containedEntity);
    }
  }

  private void OnRemoveAttachableActions(
    Entity<AttachableToggleableComponent> ent,
    ref RemoveAttachableActionsEvent args)
  {
    this.RemoveAttachableActions(ent, args.User);
    this.RemoveRelayedActions(ent, args.User);
  }

  private void RemoveAttachableActions(Entity<AttachableToggleableComponent> ent, EntityUid user)
  {
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    EntityUid valueOrDefault = action.GetValueOrDefault();
    ActionComponent comp;
    if (!this.HasComp<InstantActionComponent>(valueOrDefault) || !this.TryComp<ActionComponent>(valueOrDefault, out comp))
      return;
    EntityUid? attachedEntity = comp.AttachedEntity;
    EntityUid entityUid = user;
    if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return;
    this._actionsSystem.RemoveProvidedAction(user, (EntityUid) ent, valueOrDefault);
  }

  private void RemoveRelayedActions(
    Entity<AttachableToggleableComponent> attachable,
    EntityUid user)
  {
    ActionsContainerComponent comp;
    if (attachable.Comp.ActionsToRelayWhitelist == null || !this.TryComp<ActionsContainerComponent>(attachable.Owner, out comp))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) comp.Container.ContainedEntities)
    {
      if (this._entityWhitelistSystem.IsWhitelistPass(attachable.Comp.ActionsToRelayWhitelist, containedEntity))
        this._actionsSystem.RemoveProvidedAction(user, attachable.Owner, containedEntity);
    }
  }

  private void OnAttachableToggleAction(
    Entity<AttachableToggleableComponent> attachable,
    ref AttachableToggleActionEvent args)
  {
    args.Handled = true;
    EntityUid? holderUid;
    string slotId;
    if (!attachable.Comp.Attached || !this._attachableHolderSystem.TryGetHolder(attachable.Owner, out holderUid) || !this.TryComp<AttachableHolderComponent>(holderUid, out AttachableHolderComponent _) || !this._attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out slotId))
      return;
    AttachableToggleStartedEvent args1 = new AttachableToggleStartedEvent(holderUid.Value, args.Performer, slotId);
    this.RaiseLocalEvent<AttachableToggleStartedEvent>(attachable.Owner, ref args1);
  }

  private void OnToggleAction(
    Entity<AttachableToggleableComponent> attachable,
    ref ToggleActionEvent args)
  {
    if (!attachable.Comp.AttachedOnly || attachable.Comp.Attached)
      return;
    args.Handled = true;
  }
}
